using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models.BinaryModels.Extensions
{
    internal static class BinaryToModelExtensions
    {

        public static User ToUserModel(this UserBinary user)
            => new User()
            {
                Id = user.Id,
                Nonce = user.Nonce,
                Address = Encoding.Unicode.GetString(user.Address)
            };
    }
}
