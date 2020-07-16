﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvertiseWeb.Models.Accounts;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentity.Model;

namespace WebAdvertiseWeb.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        private readonly CognitoIdentityProviderInfo _identity;

        public AccountsController(SignInManager<CognitoUser> signManager,UserManager<CognitoUser> userManager, CognitoUserPool pool, CognitoIdentityProviderInfo identity)
        {
            _signManager = signManager;
            _userManager = userManager;
            _pool = pool;
            _identity = identity;
        }
        public async Task<IActionResult> Signup() 
        {
            var model =new SignupModel();
            return  View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null) {
                    ModelState.AddModelError("UserAlreadyExist", "This user is already exist");
                    return View(model);
                }
                user.Attributes.Add( CognitoAttribute.Email.ToString(), model.Email);
               // user.Attributes.Add(CognitoAttribute.UserName.ToString(), model.Email);
               
                     var createdUser = await  _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded) 
                {
                    RedirectToAction("Confirm");
                }
                else
                {
                    foreach (var item in createdUser.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm_Post(ConfirmModel model)
        {   
           

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with mention email is not found");
                }

                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            var model = new SignIn();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignIn model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Email or Password do not match.");
                }
            }


            return View("SignIn", model);
        }

        public async Task<IActionResult> ForgetPassword()
        {
            
            return View();
        }
    }
}
