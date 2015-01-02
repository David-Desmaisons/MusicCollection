# example StartupScript.py.  
#
# Add a file called StartupScript.py to your executable project. 
# Then, set the "Copy to Output Directory" to "Copy if newer" in the property pane for StartupScript.py.

from System import *
from System.IO import *
from System import Environment
from System.Windows import *
from System.Windows.Media import *
from System.Windows.Media.Animation import *
from System.Windows.Controls.Primitives import *
from System.Windows.Controls import *
from System.Windows.Shapes import *
from System.Globalization import *
from System.Windows.Data import *
from System.Collections.Generic import *
from System.Text import *

import clr
clr.AddReference("MusicCollection")
clr.AddReference("MusicCollectionWPF")
clr.AddReference('IronPython')
clr.AddReference('Microsoft.Scripting')

import MusicCollection
import MusicCollectionWPF
import System
clr.ImportExtensions(MusicCollection.Infra.ListLiveQueryExtension)
clr.ImportExtensions(System.Linq.Enumerable)
clr.ImportExtensions(MusicCollection.Infra.StringHelper)
clr.ImportExtensions(MusicCollection.Infra.EnumExtender)

from MusicCollection.Infra import *
from MusicCollection.Fundation import *
from MusicCollectionWPF.ViewModel import *
from MusicCollectionWPF.Infra import *
from MusicCollectionWPF.UserControls.AlbumPresenter import *


def BooleanToVisibility(bool):
    return Visibility.Visible if bool else Visibility.Collapsed
    

def BooleanToHidden(bool):
    return Visibility.Visible if bool else Visibility.Hidden


def ZPanelFromVisibility(vis):
    return 2 if (vis==Visibility.Visible) else -1


def SelectDisplay(maindisplay,player):
    return player if (maindisplay==MainDisplay.Play) else None


def BooleanToScrollBarVisibilty(bool):
    return ScrollBarVisibility.Visible if bool else ScrollBarVisibility.Hidden


def StateToOpacity(state):
    return 0.5 if (state==ObjectState.FileNotAvailable) else 1.0


def FormatPercent(percent):
	return String.Format("{0}%", percent)


def OrderGenre(genre):
	return genre.OrderBy[IGenre,String](lambda g : g.FullName).ToList()


def StaticResource(usercontrolcontext,enum):
	return usercontrolcontext.FindResource(enum.ToString())


def Translate(value,min,max,dmax,cursor):
	return  ((value - min) / (max - min)) * dmax - (cursor/2)
	

def ComplexityIsNeeded(iText,iFontFamily,iFontStyle,iFontWeight,iFontStretch,iFontSize,iActualWidth,iMaxWidth):
    if (not(type(iText)==String)):
		return False;
    Mi = iActualWidth
    if (Mi==0):
        Mi= iMaxWidth
    if ((Mi==0) or (iText==None)):
        return False
    MaxX = FormattedText(iText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface(iFontFamily, iFontStyle, iFontWeight, iFontStretch), iFontSize, Brushes.Black).Width
    return  MaxX>Mi


def TimeFormater(Time):
    return String.Format('{0:00}:{1:00}', Time.TotalMinutes,Time.Seconds)


def StringAppender(s1,s2):
    return String.Format('{0}\n{1}',s1,s2)


def AlbumToList(album):
    l = List[IAlbum]()
    l.Add(album)
    return l


def GroupIfNeeded(cond,grouped):
    return ListGrouper().AddGroup(grouped) if (cond) else ListGrouper()


def CheckedToPause(c):
    if (c==None):
        return PlayMode.Paused
    return PlayMode.Play if (c==True) else PlayMode.Paused


def IsSizeToHeigth(stc):
   return (stc==SizeToContent.WidthAndHeight or stc==SizeToContent.Height)


def IsSizeToWidth(stc):
   return (stc==SizeToContent.WidthAndHeight or stc==SizeToContent.Width)


def ConvertSizeToContentRow(c):
    return GridLength(1, GridUnitType.Auto) if IsSizeToHeigth(c) else GridLength(1, GridUnitType.Star)


def ConvertHeigthSizeToContent(stc,templateheigth):
    return Double.NaN if IsSizeToHeigth(stc) else templateheigth


def ConvertWidthSizeToContent(stc,templateheigth):
    return Double.NaN if IsSizeToWidth(stc) else templateheigth


def StringsToString(ss):
    if (ss==None):
	    return String.Empty
    return String.Join('\n', ss)


def StringToStrings(s):
    CArray = Array[Char]([ '\n', '\r' ])
    return s.Split(CArray, StringSplitOptions.RemoveEmptyEntries)


def SpaceCheckerOKStatus(sc):
    if (sc==None):
	    return 'Status KO'
    return 'Status OK' if (sc.OK==True) else 'Status KO'


def SpaceCheckerStatus(fs):
    return String.Format('Missing space {0}', -fs) if (fs.SizeInKB < 0) else String.Format('Remaing space {0}', fs)


def ExtendedAlbums(al,ap):
    if (ap==None):
	    return None
    return ap.GetSelectedEntities(al)


def Join(f,s):
    return String.Format('{0} : {1}',f, String.Empty if (s==None) else s.GetDescription())


def ContextMenuVisibility(ioa,iap,io):
    if (iap==None):
        return None
    if (ioa!=None):
        return Visibility.Visible if iap.IsCommandAllowed(ioa) else Visibility.Collapsed
    if (io==None):
        return Visibility.Collapsed
    if (not(io.Count==1)):
        return Visibility.Collapsed
    return  Visibility.Visible if isinstance(io.First(),IAlbum) else Visibility.Collapsed

def VisibleIfInList(item,list):
    return  BooleanToVisibility(item in list)

def MaxString(s,value):
    return s.ToMax(value)

def TrackToViewTrack(trs):
    if (trs==None):
	    return None
    return ListLiveQueryExtension.SelectLive[ITrack,TrackView](trs,lambda t : TrackView.GetTrackView(t))

def TrueIfCollection(mc):
    if (mc==None):
        return Visibility.Collapsed
    return BooleanToVisibility(mc.Count>1)

def CollectionNotEmpty(mc):
    if (mc==None):
        return False
    return mc.Count>=1

def VisibleIfCollectionNotEmpty(mc):
    if (mc==None):
        return Visibility.Collapsed
    return BooleanToVisibility(mc.Count>=1)

def MaxImage(Im):
    if (Im==None):
        return DependencyProperty.UnsetValue
    res = Im.GetImage(900)
    return res if (res!=None) else DependencyProperty.UnsetValue

def IsMax(value,minimum,maximum,cursor,cursormax):
    return ( ((value - minimum) * cursormax ) >=  (cursor * (maximum - minimum)))

def AlbumNumber(an):
    sb = StringBuilder(an.ToString())
    sb.Append(' Album')
    if (an>1):
        sb.Append('s')
    return sb.ToString()

def AlbumNumberAppend(str,an):
    sb = StringBuilder(an.ToString())
    sb.Append(' ')
    sb.Append(str)
    if (an>1):
        sb.Append('s')
    return sb.ToString()

def string(st):
	return st.ToString() if (st!=None) else String.Empty

def Test(v):
    s = v.Split('-')
    return [Double.Parse(s[0]),Double.Parse(s[1])]

def IsCollectionEmpty(mc):
    if (mc==None):
        return True
    return mc.Count==0

def ArtistDisplay(ar):
	sb = StringBuilder(ar.Name)
	sb.Append(' (')
	cc = ar.Albums.Count
	sb.Append(cc.ToString())
	sb.Append(' Album')
	if (cc>1):
		sb.Append('s')
	sb.Append(')')
	return sb

def AlbumDisplay(al):
	sb = StringBuilder(al.Name)
	sb.Append(' - ')
	sb.Append(al.Author)
	return sb

def SimpleTimeFormat(var):
	return "--:--" if var==None else TimeFormater(TimeSpan.FromMilliseconds(var))

def TimeFormat(curr,max):
	return String.Format('{0} / {1}',SimpleTimeFormat(curr) ,SimpleTimeFormat(max))

def TrackDisplay(tr):
	sb = StringBuilder(tr.Name)
	sb.Append('-')
	sb.Append(tr.Album.Name)
	return sb

def ToolVerOff(placement,parh,selh):
	return parh/2.0 - selh/2.0 if ( (placement == PlacementMode.Left) or (placement == PlacementMode.Right)) else float(0)

def ToolHorOff(placement,parw,selw):
	return parw/2.0 - selw/2.0 if ( (placement == PlacementMode.Top) or (placement == PlacementMode.Bottom)) else float(0)

def SortDisplay(asc):
	return 'Sort Order (Ascendant)' if asc else 'Sort Order (Descendant)'

def VolumeText(volume):
	sb = StringBuilder('Volume (')
	pour = int(volume*100)
	sb.Append(pour)
	sb.Append('%)')
	return sb.ToString()

def YearDisplay(year):
	return String.Empty if (year==0) else year

def GotoPlay(Album):
	sb = StringBuilder('Go To Album Playing')
	if (Album==None):
		return sb.ToString()
	sb.Append('\n')
	sb.Append('(')
	sb.Append(Album.Name)
	sb.Append(')')
	return sb.ToString()

def SizeText(Heigth,Width):
	sb = StringBuilder()
	sb.Append(Math.Round(Heigth))
	sb.Append(' x ')
	sb.Append(Math.Round(Width))
	return sb.ToString()

def StringAppend(Un,Deux):
	sb = StringBuilder()
	sb.Append(Un)
	sb.Append(Deux)
	return sb.ToString()

def CoverFromList(alls):
	withcovercoll = alls.Where(lambda t : t.CoverImage!=None)
	withcoveralb = withcovercoll.FirstOrDefault[IAlbum]()
	return withcoveralb.CoverImage if (withcoveralb!=None) else None

def GetDescription(enum):
	return EnumExtender.GetDescription(enum)
	
def BorderClip(aw,ah,cr):
    clip = RectangleGeometry(Rect(0, 0, aw, ah),cr.TopLeft, cr.TopLeft)
    clip.Freeze()
    return clip

def Max(un,deux):
    return un if (un>deux) else deux

def MaxText(un,deux):
    return Max( RealLengthString(un.Text,un),RealLengthString(deux.Text,deux))


def EquivalentLengthString(iText,textbox):
    if (not(type(iText)==String)):
        return 0
    sb = StringBuilder()
    sb.Append('0',iText.Length)
    return RealLengthString(sb.ToString(),textbox)+5
 
def RealLengthString(iText,textbox):
    if (not(type(iText)==String)):
        return 0
    return FormattedText(iText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface(textbox.FontFamily, textbox.FontStyle, textbox.FontWeight, textbox.FontStretch), textbox.FontSize, Brushes.Black).Width
