using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;

namespace PokemonBattleOnline
{
  public class ZipData : IDisposable
  {
    private readonly Package Pack;
    
    public ZipData(string pack)
    {
      Pack = ZipPackage.Open(new FileStream(pack, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public Stream GetStream(string path)
    {
      if (!string.IsNullOrEmpty(path) && path[0] != '/') path = "/" + path;
      return Pack.GetPart(new Uri(path, UriKind.Relative)).GetStream(FileMode.Open, FileAccess.Read);
    }

    public void Dispose()
    {
      ((IDisposable)Pack).Dispose();
    }
  }
}
