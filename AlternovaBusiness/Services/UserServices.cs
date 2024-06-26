using AlternovaBusiness.DTO;
using AlternovaBusiness.Interface;
using AlternovaBusiness.interfaces;
using AlternovaData.Entities; 

namespace AlternovaBusiness.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context; 
        private readonly IJwtHelper _jwtService; 

        public UserService(AppDbContext context, IJwtHelper jwtService)
        {
             // Assign injected AppDbContext to local _context
            _context = context; 
             // Assign injected IJwtHelper to local _jwtService
            _jwtService = jwtService;
        }

        public IEnumerable<UserDTO> Get()
        {
            return _context.User.Select(user => new UserDTO 
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email
            })
            .ToList(); 
        }

      
        public User Post(User request)
        {
            // Check if email already exists in database
            if (_context.User.Any(u => u.Email == request.Email)) 
            {
                throw new Exception("Email address is already registered."); 
            }

            try
            {
                // Hash the user's password
                request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash); 
                _context.User.Add(request); 
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create user.", ex); 
            }

            return request; 
        }

        public string Login(string email, string password)
        {
            // Find user by email in database
            var user = _context.User.FirstOrDefault(x => x.Email == email); 

            // Check if user exists and verify password using BCrypt hashing
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials"); 
            }
            // Generate JWT token for authenticated user
            return _jwtService.GenerateJwtToken(user); 
        }
    }
}
