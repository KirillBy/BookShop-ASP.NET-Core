using Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVC5_Online_shop.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;


namespace Logic.Services
{
    public class IAccountService
    {
        void CreateAccount(model)
        {
            using (Db db = new Db())
            {
                //check name for unique
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {Model.UserName} is taken.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                //create usertDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };

                //add data to userDTO model
                db.Users.Add(userDTO);

                //save data
                db.SaveChanges();

                //add role
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }
        }
    }
}
