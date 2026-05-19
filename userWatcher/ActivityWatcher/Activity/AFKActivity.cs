using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher.Activity;

public class AFKActivity : Activity
{

    public AFKActivity() : base()
    {

    }
    public override ActivityType Type 
    { 
        get 
        {
            return ActivityType.AFK;
        } 
    }
}
