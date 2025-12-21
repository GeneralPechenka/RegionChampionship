using System.Reflection.PortableExecutable;

namespace Domain.Enums
{

    public enum MachineStatusEnum { 
        Working, 
        Broken, 
        UnderMaintenance,
        OutOfService 

        
    
    }
    public static class MachineStatusEnumExtentions
    {
        public static string ToStringRu(this MachineStatusEnum status)
        {
            return status switch
            {
                MachineStatusEnum.Working => "Работает",
                MachineStatusEnum.Broken => "Сломан",
                MachineStatusEnum.UnderMaintenance => "Обслуживается",
                MachineStatusEnum.OutOfService => "Выведен из строя",
                _ => status.ToString()
            };
        }
    }
}
