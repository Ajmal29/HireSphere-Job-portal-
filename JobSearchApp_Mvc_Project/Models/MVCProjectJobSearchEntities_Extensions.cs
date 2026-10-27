using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace JobSearchApp_Mvc_Project
{
    public class sp_profile_Result
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Qualification { get; set; }
        public int? ExperienceYears { get; set; }
        public string Skills { get; set; }
    }

    public partial class MVCProjectJobSearchEntities
    {
        public virtual List<sp_profile_Result> sp_profile(int? id)
        {
            try
            {
                var result = this.Database.SqlQuery<sp_profile_Result>(
                    "EXEC sp_profile @id",
                    new SqlParameter("@id", (object)id ?? DBNull.Value)).ToList();
                if (result != null && result.Count > 0)
                {
                    return result;
                }
            }
            catch
            {
                // Fallback to Entity Framework if stored procedure has not been executed yet
            }

            var user = this.UserTables.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                return new List<sp_profile_Result>
                {
                    new sp_profile_Result
                    {
                        UserId = user.UserId,
                        Name = user.UserName ?? "",
                        Age = user.Age ?? 0,
                        Address = user.Address ?? "",
                        Email = user.Email ?? "",
                        Photo = (user.Photo ?? "").Replace("~/Photos/", ""),
                        Phone = user.Phone ?? "",
                        Qualification = user.Qualification ?? "",
                        ExperienceYears = user.ExperienceYears,
                        Skills = user.Skills ?? ""
                    }
                };
            }

            return new List<sp_profile_Result>
            {
                new sp_profile_Result
                {
                    Name = "",
                    Age = 0,
                    Address = "",
                    Email = "",
                    Photo = ""
                }
            };
        }

        public virtual int sp_profile_Update(int? id, int? age, string address, string photo = null)
        {
            try
            {
                return this.Database.ExecuteSqlCommand(
                    "EXEC sp_profile_Update @id, @age, @address, @photo",
                    new SqlParameter("@id", (object)id ?? DBNull.Value),
                    new SqlParameter("@age", (object)age ?? DBNull.Value),
                    new SqlParameter("@address", (object)address ?? DBNull.Value),
                    new SqlParameter("@photo", (object)photo ?? DBNull.Value));
            }
            catch
            {
                // Fallback to Entity Framework
                var user = this.UserTables.FirstOrDefault(u => u.UserId == id);
                if (user != null)
                {
                    user.Age = age;
                    user.Address = address;
                    if (!string.IsNullOrEmpty(photo))
                    {
                        user.Photo = photo;
                    }
                    return this.SaveChanges();
                }
                return 0;
            }
        }

        public virtual List<string> sp_selectPwd(int? id, string uname = null)
        {
            try
            {
                using (var con = new SqlConnection(this.Database.Connection.ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        if (!string.IsNullOrEmpty(uname))
                        {
                            cmd.CommandText = "SELECT TOP 1 PasswordHash FROM LoginTable WHERE Email = @uname AND IsActive = 1";
                            cmd.Parameters.AddWithValue("@uname", uname);
                        }
                        else
                        {
                            cmd.CommandText = "SELECT TOP 1 PasswordHash FROM LoginTable WHERE (UserId = @id OR LoginId = @id OR CompanyId = @id) AND IsActive = 1";
                            cmd.Parameters.AddWithValue("@id", (object)id ?? 0);
                        }
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return new List<string> { result.ToString() };
                        }
                    }
                    con.Close();
                }
            }
            catch { }

            var login = !string.IsNullOrEmpty(uname)
                ? this.LoginTables.FirstOrDefault(l => l.Email == uname)
                : this.LoginTables.FirstOrDefault(l => l.UserId == id || l.LoginId == id || l.CompanyId == id);
            if (login != null && !string.IsNullOrEmpty(login.PasswordHash))
            {
                return new List<string> { login.PasswordHash };
            }

            return new List<string> { "" };
        }

        public virtual List<string> sp_selectPwd(int? id)
        {
            return sp_selectPwd(id, null);
        }

        public virtual int sp_pwdchange(int? id, string newpwd, string uname = null)
        {
            int rows = 0;
            try
            {
                using (var con = new SqlConnection(this.Database.Connection.ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        if (!string.IsNullOrEmpty(uname))
                        {
                            cmd.CommandText = "UPDATE LoginTable SET PasswordHash = @newpwd WHERE Email = @uname OR UserId = @id OR LoginId = @id OR CompanyId = @id";
                            cmd.Parameters.AddWithValue("@uname", uname);
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE LoginTable SET PasswordHash = @newpwd WHERE UserId = @id OR LoginId = @id OR CompanyId = @id";
                        }
                        cmd.Parameters.AddWithValue("@id", (object)id ?? 0);
                        cmd.Parameters.AddWithValue("@newpwd", (object)newpwd ?? "");
                        rows = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                if (rows > 0) return rows;
            }
            catch { }

            // Fallback to Entity Framework
            try
            {
                var login = !string.IsNullOrEmpty(uname)
                    ? this.LoginTables.FirstOrDefault(l => l.Email == uname)
                    : this.LoginTables.FirstOrDefault(l => l.UserId == id || l.LoginId == id || l.CompanyId == id);
                if (login != null)
                {
                    login.PasswordHash = newpwd;
                    return this.SaveChanges();
                }
            }
            catch { }

            return 0;
        }

        public virtual int sp_pwdchange(int? id, string newpwd)
        {
            return sp_pwdchange(id, newpwd, null);
        }

        public virtual int sp_UpdateExpiredJobs(DateTime? curDate = null)
        {
            try
            {
                var p = curDate.HasValue 
                    ? new SqlParameter("@curDate", curDate.Value.Date) 
                    : new SqlParameter("@curDate", DBNull.Value);
                return this.Database.SqlQuery<int>("EXEC sp_UpdateExpiredJobs @curDate", p).FirstOrDefault();
            }
            catch
            {
                DateTime today = curDate.HasValue ? curDate.Value.Date : DateTime.Today;
                var expired = this.Jobs.Where(j => j.EndDate.HasValue && j.EndDate.Value < today && j.JobStatus != "Expired").ToList();
                foreach (var ej in expired)
                {
                    ej.JobStatus = "Expired";
                }
                return this.SaveChanges();
            }
        }
    }
}

