using B2BInAppService.Models;
using Windows.Data.Json;

namespace B2BInAppApp
{
    public static class AccessTokenCollectionDataExtension
    {
        public static JsonObject ToJson(this AccessTokenCollectionData data)
        {
            var jsonObject = new JsonObject();
            jsonObject.Add("Auth", JsonValue.CreateStringValue(data.Auth));
            jsonObject.Add("Collection", JsonValue.CreateStringValue(data.Collection));
            jsonObject.Add("Purchase", JsonValue.CreateStringValue(data.Purchase));
            return jsonObject;
        }
    }
}
