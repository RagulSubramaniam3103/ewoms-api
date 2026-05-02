using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO
{
    public class Master_UserRoleDTO
    {
        public UserRoleDTO UserRole { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRoleDTO
    {
        Admin,
        Manager,
        User
    }
}
