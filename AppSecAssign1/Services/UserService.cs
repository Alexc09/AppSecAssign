using AlexAppSecAssign.Models;
using AppSecAssign1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace AppSecAssign1.Services
{
    public class UserService
    {
        private AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            var user = _context.Users.SingleOrDefault(o => o.Email == email);
            return user;
        }

        public User GetUserById(string id)
        {
            var user = _context.Users.SingleOrDefault(o => o.Id == id);
            return user;
        }

        public Boolean UpdateUserPassword(string Id, string newPasswordHash, string newPasswordHistory)
        {
            // Password cannot be changed within x minutes of last change
            var cannotChangeBefore = 1;

            var savedUser = GetUserById(Id);
            var timeDiff = (DateTime.Now - savedUser.PasswordChangeTime).TotalMinutes;
            // Don't allow user to change password if it's before the allowed time
            if (timeDiff < cannotChangeBefore)
            {
                return false;
            }
            savedUser.Password = newPasswordHash;
            savedUser.PasswordHistory = newPasswordHistory;
            savedUser.PasswordChangeTime = DateTime.Now;
            _context.Entry(savedUser).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        public Boolean UpdateOTP(string Email, string OTP)
        {
            var savedUser = GetUserByEmail(Email);
            savedUser.OTP = OTP;
            savedUser.OTPIssuedTime = DateTime.Now;
            _context.Entry(savedUser).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        // Return true if account already exists
        public Boolean CheckAccountExists(string email)
        {
            if (_context.Users.Any(o => o.Email == email)) return true;
            return false;
        }

        public Boolean AddUser(User user)
        {
            if (CheckAccountExists(user.Email))
            {
                return false;
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        // Checks whether the lockout time is longer than x minutes. If so, set lockout=false & failedAttempts=0
        // Returns true of lockout is set to false. Else return false if lockout remains true
        public Boolean CheckLockOutStatus(string email)
        {
            User storedUser = GetUserByEmail(email);
            var timeDiff = (DateTime.Now - storedUser.AccLockoutTime).TotalMinutes;
            Console.WriteLine(timeDiff);
            // If 1 minutes have passed, unlock the account
            if (timeDiff > 1)
            {
                storedUser.FailedLoginAttempts = 0;
                storedUser.AccLockOut = false;

                _context.Entry(storedUser).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
                // Don't actually have to update AccLockOutTime, because the next time we access it (When AccLockOut = true), we will update it to that Datetime.Now
            }
            return false;
        }

        public Boolean CheckPassword(string email, string enteredPassword)
        {
            if (CheckAccountExists(email))
            {
                // To retrieve the actual hash using the Email
                User storedUser = GetUserByEmail(email);
                // If user is logged out, don't allow to proceed
                if (storedUser.AccLockOut)
                {
                    // If true, means lockout status has been set to false & loginAttempts set back to 0
                    if (CheckLockOutStatus(email))
                    {
                        return CheckPassword(email, enteredPassword);
                    }
                    // Else, the lockout status has remained as true, hence we exit the function
                    return false;
                }

                var userPasswordHash = storedUser.Password;
                var correctPassword = BC.Verify(enteredPassword, userPasswordHash);
                if (correctPassword)
                {
                    return true;
                }
                // If wrong password supplied
                else
                {
                    storedUser.FailedLoginAttempts += 1;
                    WriteToAuditLog($"{storedUser.FirstName} Failed Login attempt. Total Failed: {storedUser.FailedLoginAttempts}");
                    
                    // Update the AccLockOut field accordingly
                    if (storedUser.FailedLoginAttempts >= 3)
                    {
                        storedUser.AccLockOut = true;
                        storedUser.AccLockoutTime = DateTime.Now;
                        WriteToAuditLog($"{storedUser.FirstName} Locked Out");
                    }

                    // Remember to save to database
                    _context.Entry(storedUser).State = EntityState.Modified;
                    _context.SaveChanges();
                    return false;
                }
            }
            // If account does not exist
            else
            {
                return false;
            }
        }

        public void resetFailedLoginAttempts(string email)
        {
            User storedUser = GetUserByEmail(email);
            storedUser.FailedLoginAttempts = 0;
            _context.Entry(storedUser).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Unlike CheckPassword, this will not increment FailedLoginAttempt
        public Boolean ComparePassword(string enteredPassword, string savedPasswordHash)
        {
            return BC.Verify(enteredPassword, savedPasswordHash);
        }

        private void WriteToAuditLog(string msg)
        {
            DateTime now = DateTime.Now;
            using (StreamWriter writer = new StreamWriter("Database/AuditLog.txt", true))
            {
                writer.WriteLine($"{now} | {msg}");
            }
        }

    }
}
