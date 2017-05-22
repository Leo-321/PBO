using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PokemonBattleOnline.PBO.Converters
{
  public class IdAvatar : Converter<int>
  {
    public static readonly IdAvatar C = new IdAvatar();

    static IdAvatar()
    {
      _avatars = new Dictionary<int, BitmapImage>(214);
      for (int i = 651; i <= 867; ++i)
      {
        if (i == 790 || i == 856 || i == 857 || i == 858) continue;
        var av = ImageService.GetAvatar(i);
        _avatars.Add(i, av);
      }
    }

    private static readonly Dictionary<int, BitmapImage> _avatars;
    public static IEnumerable<int> Avatars
    { get { return _avatars.Keys; } }

    protected override object Convert(int value)
    {
      return _avatars.ValueOrDefault(value);
    }
  }
}
