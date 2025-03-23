using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Services;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly Result _result;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _result = new Result();
        }

        [HttpPost("register")]
        public async Task<ActionResult<Result>> Register(RegisterDTO registerDto)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                _result.Status = false;
                _result.Message = "Bu e-posta adresi zaten kullanımda";
                return BadRequest(_result);
            }

            var user = new IdentityUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı oluşturulurken bir hata oluştu";
                return BadRequest(_result);
            }

            // Varsayılan olarak User rolü atanır
            await _userManager.AddToRoleAsync(user, "User");

            _result.Status = true;
            _result.Message = "Kullanıcı başarıyla oluşturuldu";
            return _result;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result>> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return Unauthorized(_result);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Hatalı şifre";
                return Unauthorized(_result);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                Roles = roles.ToList()
            };

            _result.Status = true;
            _result.Message = "Giriş başarılı";
            _result.Data = userDto;

            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("roles")]
        public async Task<ActionResult<Result>> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                _result.Status = false;
                _result.Message = "Rol adı boş olamaz";
                return BadRequest(_result);
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                _result.Status = false;
                _result.Message = "Bu rol zaten mevcut";
                return BadRequest(_result);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Rol oluşturulurken bir hata oluştu";
                return BadRequest(_result);
            }

            _result.Status = true;
            _result.Message = "Rol başarıyla oluşturuldu";
            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<ActionResult<Result>> AssignRoleToUser(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return NotFound(_result);
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _result.Status = false;
                _result.Message = "Rol bulunamadı";
                return NotFound(_result);
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Rol ataması yapılırken bir hata oluştu";
                return BadRequest(_result);
            }

            _result.Status = true;
            _result.Message = "Rol başarıyla atandı";
            return _result;
        }
    }
}