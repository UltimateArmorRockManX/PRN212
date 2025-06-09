using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPatterns.HomeworkD
            paymentService.ProcessPayment("jane_doe", -50);
            paymentService.ProcessPayment("big_spender", 5000m);

            // Display logs
            Logger.GetInstance.DisplayLogs();

            // Clear logs and add more
            Logger.GetInstance.ClearLogs();
            Logger.GetInstance.LogInfo("Application shutting down");

            // Display logs again
            Logger.GetInstance.DisplayLogs();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
