using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        // 1. Login
        [HttpPost("login")]
        public async Task<ActionResult<ResultDTO>> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz giriş bilgileri";
                return BadRequest(_result);
            }

            var user = await _userManager.FindByNameAsync(loginDto.Username);

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

            // AutoMapper kullanımı (token ve roles manuel atanıyor)
            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Token = _tokenService.CreateToken(user, roles);
            userDto.Roles = roles.ToList();

            _result.Status = true;
            _result.Message = "Giriş başarılı";
            _result.Data = userDto;

            return _result;
        }

        // 2. Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz kayıt bilgileri";
                return BadRequest(_result);
            }

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı adı zaten kullanılıyor";
                return StatusCode(StatusCodes.Status400BadRequest, _result);
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                _result.Status = false;
                _result.Message = "Email adresi zaten kullanılıyor";
                return StatusCode(StatusCodes.Status400BadRequest, _result);
            }

            // Yeni kullanıcıyı manuel oluşturma (AutoMapper kullanılmıyor çünkü şifre oluşturma özel işlem)
            IdentityUser user = new()
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı oluşturma hatası: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }

            // Kullanıcıya "User" rolü atama
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            _result.Status = true;
            _result.Message = "Kullanıcı başarıyla oluşturuldu";
            return Ok(_result);
        }

        // 3. Kullanıcı Bilgileri Getirme
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return NotFound(_result);
            }

            var roles = await _userManager.GetRolesAsync(user);

            // AutoMapper kullanımı
            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Roles = roles.ToList();

            _result.Status = true;
            _result.Message = "Profil bilgileri getirildi";
            _result.Data = userDto;

            return Ok(_result);
        }

        // 4. Profil Güncelleme
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO model)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz profil bilgileri";
                return BadRequest(_result);
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return NotFound(_result);
            }

            // Email güncelleme
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                // Email zaten kullanılıyor mu kontrolü
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    _result.Status = false;
                    _result.Message = "Bu email adresi başka bir kullanıcı tarafından kullanılıyor";
                    return BadRequest(_result);
                }

                user.Email = model.Email;
            }

            // Telefon numarası güncelleme
            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Profil güncellenirken hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }

            _result.Status = true;
            _result.Message = "Profil başarıyla güncellendi";
            return Ok(_result);
        }

        // 5. Şifre Değiştirme
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz şifre bilgileri";
                return BadRequest(_result);
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return NotFound(_result);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Şifre değiştirilirken hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(_result);
            }

            _result.Status = true;
            _result.Message = "Şifre başarıyla değiştirildi";
            return Ok(_result);
        }

        // 6. Kullanıcı Listesi (Admin için)
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userListDto = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // AutoMapper kullanımı
                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Roles = roles.ToList();

                userListDto.Add(userDto);
            }

            _result.Status = true;
            _result.Message = "Kullanıcı listesi başarıyla getirildi";
            _result.Data = userListDto;
            return Ok(_result);
        }

        // 7. Kullanıcı Rolü Atama (Admin için)
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO model)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz rol bilgileri";
                return BadRequest(_result);
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bulunamadı";
                return NotFound(_result);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                _result.Status = false;
                _result.Message = "Rol bulunamadı";
                return BadRequest(_result);
            }

            // Kullanıcının mevcut rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);

            // Eğer eklenmek istenen rol zaten varsa
            if (userRoles.Contains(model.Role))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı zaten bu role sahip";
                return BadRequest(_result);
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                _result.Status = false;
                _result.Message = "Rol ataması sırasında hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }

            _result.Status = true;
            _result.Message = "Rol başarıyla atandı";
            return Ok(_result);
        }
    }
}