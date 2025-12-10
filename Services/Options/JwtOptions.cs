using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Options
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = "mN9pX7q4s8v1u3y5t7w9z2b4c6d8e0f2h4j6k8l1m3n5p7r9t1v3x5z7a";
        public int ExpireMinutes { get; set; } = 30;
    }
}
