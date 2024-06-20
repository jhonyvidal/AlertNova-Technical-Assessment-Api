using AlternovaData.Entities;

namespace AlternovaBusiness.Interface;
public interface IUserService {
    IEnumerable<User> Get();
    User Post(User request);
    string Login(string email, string password);
}