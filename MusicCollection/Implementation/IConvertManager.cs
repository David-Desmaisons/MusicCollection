using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.FileImporter;
using MusicCollection.WebServices;
using MusicCollection.WebServices.Discogs2;

namespace MusicCollection.Implementation
{
    internal interface IRarManager
    {
        void OnUnrar(string FileName, bool Successfull);

        void OnUnrar(IEnumerable<string> FileName, bool Successfull);

        IRarDescompactor InstanciateExctractorWithPassword(string FileName, IEventListener iel);

        IMccDescompactor InstanciateExctractor(string FileName, IEventListener iel, IImportContext Temp);

    }

    public interface IWebServicesManager
    {
        string FreedbServer
        {
            get;
        }

        int DiscogsTimeOut
        {
            get;
        }

        bool DiscogsActivated
        {
            get;
        }

        bool AmazonActivated
        {
            get;
        }

        string Amazon_accessKeyId
        {
            get;
        }

        string Amazon_secretKey
        {
            get;
        }    

        OAuthManager DiscogsOAuthManager
        {
            get;
        }

        DiscogsOAuthBuilder GetDiscogsOAuthBuilder();
    }
   

    //internal interface IMaturityManager
    //{
    //    bool RerootOnMaturityChange { get; }

    //    string RerootPath { get; }
    //}

    //internal interface IEmailInformationManager
    //{
    //    string EmailAdress{get;}

    //    string Password{get;}

    //    string EmailReceptor { get; }
    //}

    internal interface IXMLImportManager
    {
        bool RemoveFileOriginal { get; }
    }


    internal interface IConvertManager
    {
        void OnSourceConverted(string FileName, bool RarContext);

        string PathFromOutput(string OriginalFileName, IImportHelper Cue);
    }

    internal interface IDeleteManager
    {
        bool? DeleteFileOnDeleteAlbum
        {
            get;
        }
    }

    internal interface ImageFormatManager
    {
        byte[] GetImagetoEmbed(Stream ImageImput);

        bool IsImageRankOKToEmbed(int Rank, int Total);  
    }
}
