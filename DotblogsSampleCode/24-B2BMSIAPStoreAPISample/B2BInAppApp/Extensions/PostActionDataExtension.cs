using B2BInAppService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace B2BInAppApp
{
    public static class PostActionDataExtension
    {
        public static string Stringify(this PostActionData data)
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("AuthData", data.AuthData.ToJson());
            jsonObject.Add("CollectionStoreID", JsonValue.CreateStringValue(data.CollectionStoreID));
            jsonObject.Add("PurchaseStoreID", JsonValue.CreateStringValue(data.PurchaseStoreID));
            jsonObject.Add("UID", JsonValue.CreateStringValue(data.UID));

            return jsonObject.Stringify();
        }
    }
}
