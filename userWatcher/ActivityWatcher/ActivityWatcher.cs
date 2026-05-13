using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher;

public class ActivityWatcher
{
    private static ActivityWatcher? _instance;
    public static ActivityWatcher? SharedController()
    {
        if (_instance == null)
        {
            _instance = new ActivityWatcher();
        }
        return _instance;
    }
    private ActivityWatcher()
    {

    }

    public Activity.Activity GetCurrentActivity()
    {
        Activity.Activity result = new Activity.Activity();

        return result;
    }
}
