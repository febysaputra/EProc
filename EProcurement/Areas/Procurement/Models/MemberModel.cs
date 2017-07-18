using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EProcurement.Models;

namespace EProcurement.Areas.Procurement.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        protected DBconn db = new DBconn();
        public Boolean CheckMembership(string username, string password)
        {
            Boolean res = false;
            String pwd = "";
            String slt = "";

            db.Open();
            SqlDataReader db_select = db.DataSelect("SELECT id_user, password, id_group FROM Mst_user WHERE username = '" + username + "'").ExecuteReader(CommandBehavior.CloseConnection);

            if (db_select.Read())
            {
                if (db_select["id_user"] != null)
                {
                    res = true;
                    pwd = db_select["password"].ToString();
                }
            }

            db_select.Close();

            if (res == true)
            {
                slt = pwd.Substring(3, 6);

                MemberModel mdl = new MemberModel();
                string member_pwd = mdl.get_member_hash_pwd(slt, password);

                if (member_pwd == pwd)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }

            db_select.Close();
            db.Close();

            return res;
        }
    }

    public class ChangePasswordModel
    {
        [Display(Name = "ID User")]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation new password do not match.")]
        public string ConfirmNewPassword { get; set; }

        protected DBconn db = new DBconn();

        public int ChangePassword(string username, string newP, string oldP, int pic_input)
        {
            int result = 0;
            LoginModel lm = new LoginModel();
            MemberModel mm = new MemberModel();

            if (lm.CheckMembership(username, oldP))
            {
                result = 1;

                try
                {
                    db.Open();
                    string query_update = "UPDATE Mst_user SET password = '" + mm.get_member_hash_pwd(mm.genSalt(username), newP) + "', change_date = '" + DateTime.Now + "', change_by = " + pic_input + " WHERE username = '" + username + "'";
                    int i = db.DataEdit(query_update);

                    result = 2;
                }
                catch
                {
                    result = -1;
                }
                finally
                {
                    db.Close();
                }
            }


            return result;
        }
    }

    public class ChangeUsernameModel
    {
        [Display(Name = "ID User")]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Old Username is required")]
        [Display(Name = "Old Username")]
        public string OldUsername { get; set; }

        [Required(ErrorMessage = "New Username is required")]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }


        protected DBconn db = new DBconn();

        public int ChangeUsername(string newU, string oldU, int pic_input)
        {
            int result = 0;
            LoginModel lm = new LoginModel();
            MemberModel mm = new MemberModel();

            String pwd = "";

            db.Open();
            SqlDataReader db_select = db.DataSelect("SELECT id_user, password, id_group FROM Mst_user WHERE username = '" + oldU + "'").ExecuteReader(CommandBehavior.CloseConnection);

            if (db_select.Read())
            {
                if (db_select["id_user"] != null)
                {
                    pwd = db_select["password"].ToString();
                }
            }

            db_select.Close();

            if (lm.CheckMembership(oldU, pwd))
            {
                result = 1;

                try
                {
                    db.Open();
                    string query_update = "UPDATE Mst_user SET password = '" + mm.get_member_hash_pwd(mm.genSalt(newU), pwd) + "', username = '" + newU + "', change_date = '" + DateTime.Now + "', change_by = " + pic_input + " WHERE username = '" + oldU + "'";
                    int i = db.DataEdit(query_update);

                    result = 2;
                }
                catch
                {
                    result = -1;
                }
                finally
                {
                    db.Close();
                }
            }


            return result;
        }
    }

    public class MemberModel
    {
        [Display(Name = "ID User")]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[a-zA-Z0-9_.]*$", ErrorMessage="Username is not valid")]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Nama is required")]
        [Display(Name = "Nama")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Group is required")]
        [Display(Name = "ID Group")]
        public int Id_group { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Tanggal Register")]
        public DateTime Register_date { get; set; }

        protected DBconn db = new DBconn();

        public List<MemberModel> get_member_list(string filter, string page_limit)
        {
            List<MemberModel> member_list = new List<MemberModel>();

            db.Open();
            SqlDataReader db_select = db.DataSelect("SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY nama ASC) AS NUMBER, * FROM Mst_user " + filter + ") AS TBL " + page_limit).ExecuteReader(CommandBehavior.CloseConnection);
            while (db_select.Read())
            {
                member_list.Add(new MemberModel
                {
                    IdUser = Convert.ToInt32(db_select["id_user"]),
                    Username = db_select["username"].ToString(),
                    Name = db_select["name"].ToString(),
                    Email = db_select["email"].ToString(),
                    Id_group = Convert.ToInt32(db_select["id_group"]),
                    Status = Convert.ToInt32(db_select["status"]),
                    Register_date = Convert.ToDateTime(db_select["created_date"])
                });
                
            }

            db_select.Close();
            db.Close();

            return member_list;
        }

        public MemberModel get_member_detail(string username)
        {
            MemberModel mbr = new MemberModel();

            db.Open();
            SqlDataReader db_select = db.DataSelect("SELECT * FROM Mst_user WHERE username ='" + username + "'").ExecuteReader(CommandBehavior.CloseConnection);
            if (db_select.Read())
            {
                mbr.IdUser = Convert.ToInt32(db_select["id_user"]);
                mbr.Username = db_select["username"].ToString();
                mbr.Password = db_select["password"].ToString();
                mbr.Name = db_select["name"].ToString();
                mbr.Email = db_select["email"].ToString();
                mbr.Id_group = Convert.ToInt32(db_select["id_group"]);
                mbr.Status = Convert.ToInt32(db_select["status"]);
                mbr.Register_date = Convert.ToDateTime(db_select["created_date"]);
            }

            db_select.Close();
            db.Close();

            return mbr;
        }

        public MemberModel get_member_detail_by_id(int id)
        {
            MemberModel mbr = new MemberModel();

            db.Open();
            SqlDataReader db_select = db.DataSelect("SELECT * FROM Mst_user WHERE id =" + id + "").ExecuteReader(CommandBehavior.CloseConnection);
            if (db_select.Read())
            {
                mbr.IdUser = Convert.ToInt32(db_select["id_user"]);
                mbr.Username = db_select["username"].ToString();
                mbr.Password = db_select["password"].ToString();
                mbr.Name = db_select["name"].ToString();
                mbr.Email = db_select["email"].ToString();
                mbr.Id_group = Convert.ToInt32(db_select["id_group"]);
                mbr.Status = Convert.ToInt32(db_select["status"]);
                mbr.Register_date = Convert.ToDateTime(db_select["created_date"]);
            }

            db_select.Close();
            db.Close();

            return mbr;
        }

        public string genSalt(string username)
        {
            var bytes = new UTF8Encoding().GetBytes(username + DateTime.Now.ToString("yyyyMMddHHmmsss"));
            byte[] hashBytes;
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }

            return Convert.ToBase64String(hashBytes).Substring(0, 6);
        }

        public string hashPass(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            byte[] hashBytes;
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }

        public string get_member_hash_pwd(string slt, string password)
        {
            string hash_pwd = hashPass(slt + password);

            string comb_pwd = hash_pwd.Substring(0, 3) + slt + hash_pwd.Substring(3);

            return comb_pwd;
        }

        public Boolean checkUsername(string user_name)
        {
            Boolean result = false;

            db.Open();
            SqlDataReader dr = db.DataSelect("SELECT COUNT(id) as jum FROM Mst_user WHERE username = '" + user_name + "'").ExecuteReader(CommandBehavior.CloseConnection);
            if (dr.Read())
            {
                if (Convert.ToInt32(dr["jum"]) < 1)
                {
                    result = true;
                }
            }
            dr.Close();

            return result;
        }

        public Boolean insertMember(MemberModel mbr, int pic_input) //aktif ketika vendor di approve
        {
            Boolean result = false;

            db.Open();
            SqlDataReader dr = db.DataSelect("SELECT COUNT(id) as jum FROM Mst_user WHERE username = '" + mbr.Username + "'").ExecuteReader(CommandBehavior.CloseConnection);
            if (dr.Read())
            {
                if (Convert.ToInt32(dr["jum"]) < 1)
                {
                    result = true;
                }
            }

            if (result)

            {
                try
                {
                    db.Open();
                    string pwd = get_member_hash_pwd(genSalt(mbr.Username), mbr.Password);
                    string query_insert = "INSERT INTO Mst_user(id_group, username, password, name, email, status, created_date, change_date, change_by) VALUES(" + mbr.Id_group + ", '" + mbr.Username + "', '" + pwd + "', '" + mbr.Name + "', '" + mbr.Email + "', " + mbr.Status + ", '" + DateTime.Now + "', '" + DateTime.Now + "', " + pic_input + ")";
                    int i = db.DataInsert(query_insert);

                    if (i > 0)
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                }
                
            }

            db.Close();

            return result;
        }

        public Boolean editMember(MemberModel mbr, int pic_input) //hanya profilenya saja
        {
            Boolean result = false;

            try
            {
                db.Open();
                string query_update = "UPDATE Mst_user SET id_group = " + mbr.Id_group + ", name = '" + mbr.Name + "', email = '" + mbr.Email + "', status = " + mbr.Status + ", change_date = '" + DateTime.Now + "', change_by = " + pic_input + " WHERE id = " + mbr.IdUser;
                int i = db.DataEdit(query_update);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }


            db.Close();

            return result;
        }

        public Boolean deleteMember(int id)
        {
            Boolean result = false;

            try
            {
                db.Open();
                string query_delete = "DELETE FROM users WHERE id = " + id;
                int i = db.DataDelete(query_delete);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }


            db.Close();

            return result;
        }
    }
}