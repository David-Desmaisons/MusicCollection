Music Collection
==============

Description
--------------
MusicCollection is a C# application that allows to manage and play digital music.
It allows to organise our music files connecting to webservices and downloading music cover art.
MusicCollection is able also to import zip and rar files, convert files to mp3. and has facilities to export music.


Instalation
--------------

In order to have a running application, you should request un4seen Bass keys (see: http://www.un4seen.com/),
AWSECommerceService keys from Amazon services (see http://docs.aws.amazon.com/AWSECommerceService/latest/DG/Welcome.html)
and discogs keys (http://www.discogs.com/developers/).

As MusicCollection uses un4seen Bass as an infrastructure to play music, MusicCollection will not run.

Then you should provide in MusicCollectionWPF root folder a file named *appsecretprivatekeys.config* containing the 
correspondings keys:

	<appSettings>
  		<add key="AmazonaccessKeyId" value="..."/>
  		<add key="AmazonsecretKey" value="..."/>
  		<add key="DiscogsConsumerKey" value="..."/>
  		<add key="DiscogsConsumerSecret" value="..."/>
  		<add key="BassPassword" value="..."/>
  		<add key="BassUser" value="..."/>
	</appSettings>