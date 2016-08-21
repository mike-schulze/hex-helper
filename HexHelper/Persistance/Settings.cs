using HexHelper.Libs.Persistance;

namespace HexHelper.Persistance
{
    public sealed class Settings : ISettings
    {
        public string LastUser
        {
            get
            {
                return Properties.Settings.Default.LastUser;
            }

            set
            {
                Properties.Settings.Default.LastUser = value;
                Save();
            }
        }

        private void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
