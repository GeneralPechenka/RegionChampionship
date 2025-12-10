namespace Domain.Enums
{
    public enum EmployeeRoleEnum { Admin, Franchisee, Operator, Technician, Manager }
    public static class EmployeeRoleEnumExtensions
    {
        public static string ToStringRu(this EmployeeRoleEnum role)
        {
            return role switch
            {
                EmployeeRoleEnum.Admin => "Администратор",
                EmployeeRoleEnum.Franchisee => "Франчайзи",
                EmployeeRoleEnum.Operator => "Оператор",
                EmployeeRoleEnum.Technician => "Техник",
                EmployeeRoleEnum.Manager => "Менеджер",
                _ => role.ToString()
            };
        }
    }
}
