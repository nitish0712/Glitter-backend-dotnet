using AutoMapper;
using GlitterTweeting.Data.DB_Context;
using GlitterTweeting.Shared;
using GlitterTweeting.Shared.DTO.Relationship;
using GlitterTweeting.Shared.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlitterTweeting.Business.Business_Objects
{
   public class UserBusinessContext : IDisposable
    {
        private UserDBContext UserDBContext;
        private IMapper UserMapper;


        public UserBusinessContext()
        {
            UserDBContext = new UserDBContext();
            var userMappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRegisterDTO, UserCompleteDTO>();
            });
            UserMapper = new Mapper(userMappingConfig);
        }

        public async Task<UserCompleteDTO> CreateNewUser(UserRegisterDTO userInput)
        {
            
                if (UserDBContext.EmailExists(userInput.Email))
                {
                    userInput.Password = PasswordHasher.HashPassword(userInput.Password);
                    if (userInput.Image == null)
                    {
                        userInput.Image = Constants.DEFAULT_USER_IMAGE;
                    }
                    Guid ID = await UserDBContext.CreateNewUser(userInput);

                    UserCompleteDTO user;
                    user = UserMapper.Map<UserRegisterDTO, UserCompleteDTO>(userInput);
                    user.ID = ID;
                    return user;
                }
                else
                {
                    throw new Exceptions.AlreadyExistsException("Email address already in use");
                }
           
        }

        
        public async Task<UserCompleteDTO> LoginUserCheck(UserLoginDTO userLoginDTO)
        {
            UserAuthDTO userAuthInfo = UserDBContext.GetCredentialsByEmail(userLoginDTO.Email);
            if (userAuthInfo == null)
            {
                throw new Exceptions.InvalidCredentialsException("Email not found");
            }
            if (PasswordHasher.ValidatePassword(userLoginDTO.Password, userAuthInfo.Password))
            {
                UserCompleteDTO userCompleteDTO = await UserDBContext.GetUserCompleteInfo(userAuthInfo);
                return userCompleteDTO;
            }
            else
            {
                throw new Exceptions.InvalidCredentialsException("Password is Incorrect");
            }
        }

        public bool UnFollow(FollowDTO followdto)
        {
            UserDBContext.UnFollow(followdto);
            return true;
        }
        public bool Follow(FollowDTO followdto)
        {
            bool result = UserDBContext.Follow(followdto);
            return result;
        }


        public IList<UserBasicDTO> GetAllFollowers(Guid loggedinuserid)
        {
            IList<UserBasicDTO> gdto = UserDBContext.GetAllFollowers(loggedinuserid);
            return gdto;
        }

        public IList<UserBasicDTO> GetAllFollowing(Guid loggedinuserid)
        {
            IList<UserBasicDTO> gdto = UserDBContext.GetAllFollowing(loggedinuserid);
            return gdto;
        }

      
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserDBContext != null)
                {
                    UserDBContext.Dispose();
                }
            }
        }

        
        ~UserBusinessContext()
        {
            Dispose(false);
        }



    }
}
