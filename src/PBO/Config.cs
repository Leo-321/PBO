using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace PokemonBattleOnline.PBO
{
    [DataContract(Namespace = PBOMarks.PBO)]
    public class Config
    {
        public static Config Current
        { get; private set; }

        public static void Load(string path)
        {
            try
            {
                using (FileStream f = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    Current = Serializer.Deserialize<Config>(f);
            }
            catch
            {
                Current = new Config();
            }
            Current.Path = path;
        }

        private string Path;

        [DataMember(EmitDefaultValue = false)]
        public int PokemonNumber;

        [DataMember(Name = "Servers", EmitDefaultValue = false)]
        private List<string> _servers;
        public List<string> Servers
        {
            get
            {
                if (_servers == null) _servers = new List<string>(30);
                return _servers;
            }
            set { _servers = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        public string Name;

        [DataMember(EmitDefaultValue = false)]
        public int Avatar;

        private Config()
        {
        }

        public void Save()
        {
            using (FileStream f = new FileStream(Path, FileMode.Create))
                Serializer.Serialize(this, f);
        }
    }
}
