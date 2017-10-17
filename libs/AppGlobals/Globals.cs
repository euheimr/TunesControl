using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AppGlobals
{
    public class Globals
    {

        //main
        public static string AppName = Convert.ToString(Assembly.GetExecutingAssembly().GetName());
        public static string AppVersion = Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version);
        public static string AppNameVer = AppName + " v" + AppVersion;

        public static string userName;
        public static string password;

        //TODO: move to a song/playlist control library..
        public static string[] playlist;
        public static string currentPlaylist;
        public static string currentAlbum;
        public static string currentSong;





    }
}
