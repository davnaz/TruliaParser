using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruliaParser.Components
{
    public class Constants
    {
        public class OfferPageSelectors
        {
            public const string phone = ".property_contact_field.h7.man";
            public const string idealIncome  = "document.querySelector('span.typeEmphasize.h2').innerText"; // format $0-9
            public const string description = "document.querySelector('span[itemprop=description]').innerText";
            public const string metaInfo = "";
            public const string  = "";
            public const string  = "";
            public const string  = "";
            public const string  = "";
            public const string  = "";
        }

        public class WebAttrsNames
        {
            public const string href = "href";
            public const string NotFound = "no";            
        }
        public class OfferJSObjectKeys
        {
            public const string id = "id";
            public const string agentName = "agentName";
            public const string apartmentNumber = "apartmentNumber";
            public const string city = "city";
            public const string county = "county";
            public const string countyFIPS = "countyFIPS";
            public const string dataPhotos = "dataPhotos";
            public const string feedId = "feedId";
            public const string formattedBedAndBath = "formattedBedAndBath";
            public const string formattedPrice = "formattedPrice";
            public const string formattedSqft = "formattedSqft";
            public const string hasPhotos = "hasPhotos";
            public const string isRentalCommunity = "isRentalCommunity";
            public const string latitude = "latitude";
            public const string longitude = "longitude";
            public const string locationId = "locationId";
            public const string listingId = "listingId";
            public const string numBathrooms = "numBathrooms";
            public const string numBedrooms = "numBedrooms";
            public const string numBeds = "numBeds";
            public const string numFullBathrooms = "numFullBathrooms";
            public const string numPartialBathrooms = "numPartialBathrooms";
            public const string pdpURL = "pdpURL";
            public const string price = "price";
            public const string truliaRank = "truliaRank";
            public const string type = "type";
            public const string zipCode = "zipCode";
            public const string streetNumber = "streetNumber";
            public const string thumbnail = "thumbnail";
            public const string sqft = "sqft";
            public const string stateCode = "stateCode";
            public const string stateName = "stateName";
            public const string street = "street";
        }

        public class OfferListSelectors
        {
            public const string NextPage = ".paginationContainer .mrs.bas.pvs.phm:nth-last-child(2)";
            public const string OfferLinks = "a.tileLink.phm";
        }


        public class OfferCellNames
        {
            public const string City = "@City";
            public const string PostID = "@PostID"; 
            public const string Name = "@Name"; 
            public const string Price = "@Price"; 
            public const string PlaceName = "@Place";
            public const string PlaceMapsLink = "@Placemaplink";
            public const string Description = "@Description"; 
            public const string BedRooms = "@Bedrooms"; 
            public const string BathRooms = "@Bathrooms";
            public const string Square = "@Square";
            public const string Availability = "@Availability";
            public const string Images = "@Images";
            public const string Additional = "@Additional"; 
            public const string Posted = "@Posted"; 
            public const string Updated = "@Updated"; 
        }

        public class RegionLinkDbParams
        {
            public const string State = "@State";
            public const string RegionName = "@RegionName";
            public const string Link = "@Link";            
        }

    }
}
