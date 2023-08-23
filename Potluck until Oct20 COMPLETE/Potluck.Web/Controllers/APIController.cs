using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Potluck.Web.Infrastructure;
using Potluck.Web.Models;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// The APIController class contains all the methods for interfacing with the back end api. 
    /// </summary>
    public class APIController
    {
        
        private string ServerAddress = "https://www.pot-luck.co:8167/";
        public string Token { get; set; }

        public string googleApiKey = "AIzaSyBXhC0GFJXFH9KATR8cNMGp9kqYohryOEU";

        public APIController()
        {
            ;
        }

        public APIController(string token)
        {
            Token = token;
        }
        /// <summary>
        /// Returns a google geocode object
        /// Used for the conversion of addresses to gps co-ordinates
        /// </summary>
        /// <param name="address">String</param>
        /// <returns>GoogleGeo Object</returns>
        public async Task<GoogleGeo> GetCoords(string address)
        {
            GoogleGeo response = new GoogleGeo();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                //HTTP GET
                var responseTask = client.GetAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={googleApiKey}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<GoogleGeo>();
                    readTask.Wait();

                    response = readTask.Result;
                }
            }
            return response;
        }

        ////////////////
        //Auth Actions//
        ////////////////

        public async Task<bool> ForgotPassword(string email)
        {
            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();

                //HTTP POST
                var postTask = client.PostAsJsonAsync<String>($"/api/v1/auth/forgot-password/{email}", "");
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    var readTask = result.Content.ReadAsAsync<UserDTO>();
                    readTask.Wait();
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Sends traditional signup data to back end
        /// </summary>
        /// <param name="signUp">SignUpRequest</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> SignUpUser(SignUpRequest signUp)
        {
            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();

                //HTTP POST
                var postTask = client.PostAsJsonAsync<SignUpRequest>("/api/v1/auth/signup", signUp);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    var readTask = result.Content.ReadAsAsync<UserDTO>();
                    readTask.Wait();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sends login details to back end for traditional login
        /// </summary>
        /// <param name="login">LoginRequest</param>
        /// <returns>Auth token string</returns>
        public async Task<string> LoginUser(LoginRequest login)
        {
            string token = "";
            Dictionary<string, string> responseDict = new Dictionary<string, string>();
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();

                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginRequest>("/api/v1/auth/login", login);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                    token = responseDict["accessToken"];
                }
            }

            return token;
        }

        //////////////////
        //Config Actions//
        //////////////////

        /// <summary>
        /// Returns a category object when passed a categoryId string
        /// </summary>
        /// <param name="categoryId">String</param>
        /// <returns>Returns a category object</returns>
        public async Task<Category> GetCategoryById(string categoryId)
        {
            Category category = new Category();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/config/category/{categoryId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Category>();
                    readTask.Wait();

                    category = readTask.Result;
                }
            }

            return category;
        }

        /// <summary>
        /// Delete a category in the back end
        /// </summary>
        /// <param name="categoryId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteCategory(string categoryId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/config/category/{categoryId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Passes CategoryVO object to back end for creation
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> CreateCategory(Category category)
        {
            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<Category>("/api/v1/config/category/create", category);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Passes a CategoryVO to back end for update
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateCategory(Category category)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<Category>($"/api/v1/config/category/update", category);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Deletes a StateCountry from back end
        /// </summary>
        /// <param name="stateCountryId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteStateCountry(string stateCountryId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/config/state-country/{stateCountryId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves StateCountry object from back end by passing id string
        /// </summary>
        /// <param name="stateCountryId">String</param>
        /// <returns>StateCountry object</returns>
        public async Task<StateCountry> GetStateCountryById(string stateCountryId)
        {
            StateCountry stateCountry = new StateCountry();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/config/state-country/{stateCountryId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<StateCountry>();
                    readTask.Wait();

                    stateCountry = readTask.Result;
                }
            }

            return stateCountry;
        }

        /// <summary>
        /// Passes StateCountry object to back end for creation
        /// </summary>
        /// <param name="stateCountry">StateCountry</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> CreateStateCountry(StateCountry stateCountry)
        {
            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<StateCountry>("/api/v1/config/state-country/create", stateCountry);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates StateCountry by passing StateCountry to back end
        /// </summary>
        /// <param name="stateCountry">StateCountry</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateStateCountry(StateCountry stateCountry)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<StateCountry>($"/api/v1/config/state-country/update", stateCountry);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }

                return false;
            }
        }

        //////////////////
        //Custom Actions//
        //////////////////
        
        /// <summary>
        /// Creates a new custom by passing a CustomVO to backend, returns a string id to new custom
        /// </summary>
        /// <param name="custom">CustomVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateCustom(CustomVO custom)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<CustomVO>("/api/v1/custom/create", custom);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("customId");
        }

        /// <summary>
        /// Returns CustomDTO from backend when provided with a string id
        /// </summary>
        /// <param name="customId">String</param>
        /// <returns>CustomDTO</returns>
        public async Task<CustomDTO> GetCustomById(string customId)
        {
            CustomDTO customResponse = new CustomDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/custom/{customId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<CustomDTO>();
                    readTask.Wait();

                    customResponse = readTask.Result;
                }
            }
            return customResponse;
        }

        /// <summary>
        /// Returns all of a users customs as a page from the back end
        /// </summary>
        /// <param name="pageNumber">Long</param>
        /// <param name="size">Long</param>
        /// <returns>PageOfCustoms</returns>
        public async Task<PageOfCustoms> GetAllCustoms(long pageNumber = 0, long size = 8)
        {
            PageOfCustoms pageOfCustoms = new PageOfCustoms();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/custom/getAll?page={pageNumber}&size={size}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfCustoms>();
                    readTask.Wait();

                    pageOfCustoms = readTask.Result;
                }
            }
            return pageOfCustoms;
        }

        /// <summary>
        /// Adds an item to the speicifed custom
        /// </summary>
        /// <param name="customId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> AddItemToCustom(string customId, string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/custom/{customId}/addItem/{itemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes and item from the specified custom
        /// </summary>
        /// <param name="customId"></param>
        /// <param name="itemId"></param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveItemFromCustom(string customId, string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/custom/{customId}/removeItem/{itemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes a custom from the back end by id
        /// </summary>
        /// <param name="customId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteCustom(string customId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/custom/{customId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Passes CustomVO to be updated to back end
        /// </summary>
        /// <param name="custom">CustomVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateCustom(CustomVO custom)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/custom/update", custom);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////////
        //Delivery Actions//
        ////////////////////

        /// <summary>
        /// Delete delivery object from back end by id
        /// </summary>
        /// <param name="deliveryId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteDelivery(string deliveryId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/delivery/{deliveryId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns Delivery object from back end by id
        /// </summary>
        /// <param name="deliveryId">String</param>
        /// <returns>DeliveryDTO</returns>
        public async Task<DeliveryDTO> GetDeliveryById(string deliveryId)
        {
            DeliveryDTO delivery = new DeliveryDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/delivery/{deliveryId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<DeliveryDTO>();
                    readTask.Wait();

                    delivery = readTask.Result;
                }
            }
            return delivery;
        }

        /// <summary>
        /// Creates delivery object in back end by passing DeliveryVO
        /// </summary>
        /// <param name="delivery">DeliveryVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateDelivery(DeliveryVO delivery)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<DeliveryVO>("/api/v1/delivery/create", delivery);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("deliveryId");
        }

        /// <summary>
        /// Returns a page of delivery objects based on the user email
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>PageOfDeliveries</returns>
        public async Task<PageOfDeliveries> GetAllDeliveriesByEmail(string email)
        {
            PageOfDeliveries pageOfDeliveries = new PageOfDeliveries();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/delivery/get-by/{email}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfDeliveries>();
                    readTask.Wait();

                    pageOfDeliveries = readTask.Result;
                }
            }
            return pageOfDeliveries;
        }

        /// <summary>
        /// Updates delivery object by passing DeliveryVO
        /// </summary>
        /// <param name="delivery">DeliveryVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateDelivery(DeliveryVO delivery)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/delivery/update", delivery);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        ////////////////
        //Item Actions//
        ////////////////

        /// <summary>
        /// Creates item object in back end by passing ItemVO
        /// </summary>
        /// <param name="item">ItemVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateItem(ItemVO item)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<ItemVO>("/api/v1/item/create", item);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("itemId");
        }

        /// <summary>
        /// Returns ItemDTO object from back end by passing id
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>ItemDTO</returns>
        public async Task<ItemDTO> GetItemById(string itemId)
        {
            ItemDTO itemResponse = new ItemDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/item/{itemId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ItemDTO>();
                    readTask.Wait();

                    itemResponse = readTask.Result;
                }
            }
            return itemResponse;
        }

        /// <summary>
        /// Returns all of a users items as a page by passing userID
        /// </summary>
        /// <param name="userId">String</param>
        /// <param name="pageNumber">Long</param>
        /// <param name="size">Long</param>
        /// <returns>PageOfItems</returns>
        public async Task<PageOfItems> GetMenuByUserId(string userId, long pageNumber = 0, long size = 8)
        {
            PageOfItems pageOfItems = new PageOfItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/item/getbyuserid/{userId}?page={pageNumber}&size={size}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<PageOfItems>();
                    readTask.Wait();
                    pageOfItems = readTask.Result;

                }
            }
            return pageOfItems;
        }

        /// <summary>
        /// Returns search results based on search term, location, and local time
        /// </summary>
        /// <param name="search">String</param>
        /// <param name="latitude">String</param>
        /// <param name="longitude">String</param>
        /// <param name="pageNumber">Long</param>
        /// <param name="time">String</param>
        /// <returns>PageOfItems</returns>
        public async Task<PageOfItems> GetMenuBySearch(string search, double latitude, double longitude, long pageNumber = 0, string time="00:00")
        {
            PageOfItems pageOfItems = new PageOfItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/item/search/{search}?page={pageNumber}&size=8&time={time}&latitude={latitude}&longitude={longitude}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<PageOfItems>();
                    readTask.Wait();
                    pageOfItems = readTask.Result;
                }
            }
            return pageOfItems;
        }

        /// <summary>
        /// Returns all Categories
        /// </summary>
        /// <returns>List<Category></returns>
        public async Task<List<Category>> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/item/getallcategories");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<List<Category>>();
                    readTask.Wait();
                    categories = readTask.Result;
                }
            }
            return categories;
        }

        /// <summary>
        /// Updates an item by passing an ItemVO
        /// </summary>
        /// <param name="item">ItemVO</param>
        /// <returns>String</returns>
        public async Task<string> UpdateItem(ItemVO item)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<ItemVO>($"/api/v1/item/update", item);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("itemId");
        }

        /// <summary>
        /// Deletes item by passing item id
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteItem(string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/item/{itemId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a custom object to an item object by passing item id and custom id
        /// </summary>
        /// <param name="itemId">String</param>
        /// <param name="customId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> AddCustomToItem(string itemId, string customId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/item/{itemId}/addcustom/{customId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a custom object from an item object by passing item id and custom id
        /// </summary>
        /// <param name="itemId">String</param>
        /// <param name="customId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveCustomFromItem(string itemId, string customId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/item/{itemId}/removecustom/{customId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Allows a seller to toggle the availablity of an item by passing the itme id
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> ItemEnableSwitch(string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/item/{itemId}/switch-enable", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /////////////////
        //Order Actions//
        /////////////////

        /// <summary>
        /// Returns an item by passing item id
        /// </summary>
        /// <param name="orderId">String</param>
        /// <returns>OrderDTO</returns>
        public async Task<OrderDTO> GetOrderById(string orderId)
        {
            OrderDTO orderResponse = new OrderDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/order/{orderId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<OrderDTO>();
                    readTask.Wait();

                    orderResponse = readTask.Result;
                }
            }
            return orderResponse;
        }

        /// <summary>
        /// Returns a page of orders from the buyers perspective
        /// </summary>
        /// <param name="status">String</param>
        /// <param name="pageNumber">Long</param>
        /// <param name="pageSize">Long</param>
        /// <returns>PageOfOrders</returns>
        public async Task<PageOfOrders> GetOrdersOfBuyer(string status = "OPENED", long pageNumber = 0, long pageSize = 8)
        {
            PageOfOrders pageOfOrders = new PageOfOrders();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/order/ofthebuyer?orderStatus={status}&page={pageNumber}&size={pageSize}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfOrders>();
                    readTask.Wait();

                    pageOfOrders = readTask.Result;
                }
            }
            return pageOfOrders;
        }

        /// <summary>
        /// Returns a page of orders from the sellers perspective
        /// </summary>
        /// <param name="status">String</param>
        /// <param name="pageNumber">Long</param>
        /// <param name="pageSize">Long</param>
        /// <returns>PageOfOrders</returns>
        public async Task<PageOfOrders> GetOrdersOfSeller(string status = "OPENED", long pageNumber = 0, long pageSize = 8)
        {
            PageOfOrders pageOfOrders = new PageOfOrders();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/order/oftheseller?orderStatus={status}&page={pageNumber}&size={pageSize}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfOrders>();
                    readTask.Wait();

                    pageOfOrders = readTask.Result;
                }
            }
            return pageOfOrders;
        }

        /// <summary>
        /// Creates an order by passing OrderVO
        /// </summary>
        /// <param name="order">OrderVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateOrder(OrderVO order)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<OrderVO>("/api/v1/order/create", order);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("orderId");
        }

        /// <summary>
        /// Adds an item to an order by passing an orderid and an OrdersItemVO
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="ordersItem">OrdersItemVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> AddOrderItemToOrder(string orderId, OrdersItemVO ordersItem)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<OrdersItemVO>($"/api/v1/order/{orderId}/add-orderitem", ordersItem);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes an item from and order by passing the order id and orderitem id
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="orderItemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveOrderItemFromOrder(string orderId, string orderItemId)
        {
            int statusCode;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP PUT
                var putTask = client.PutAsJsonAsync<string>($"/api/v1/order/{orderId}/remove-orderitem/{orderItemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveItemFromOrder(string orderId, string itemId)
        {
            int statusCode;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP PUT
                var putTask = client.PutAsJsonAsync<string>($"/api/v1/order/{orderId}/removeItem/{itemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates an order by passing OrderVO
        /// </summary>
        /// <param name="order">OrderVO</param>
        /// <returns>String</returns>
        public async Task<string> UpdateOrder(OrderVO order)
        {
            int statusCode;
            Dictionary<string, string> responseDict = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP PUT
                var putTask = client.PutAsJsonAsync<OrderVO>($"/api/v1/order/update", order);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string, string>>();
                    readTask.Wait();
                    responseDict = readTask.Result;
                }
            }
            return responseDict.GetValueOrDefault("orderId");
        }

        /// <summary>
        /// Changes order status by passing order id and status
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="newStatus">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> ChangeOrderStatus(string orderId, string newStatus)
        {
            int statusCode;
            Dictionary<string, long> responseDict = new Dictionary<string, long>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP PUT
                var putTask = client.PutAsJsonAsync<string>($"/api/v1/order/{orderId}/changestatus/{newStatus}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes an order by passing order id
        /// </summary>
        /// <param name="orderId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteOrder(string orderId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/order/{orderId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        //////////////////
        //Review Actions//
        //////////////////

        /// <summary>
        /// Deletes a review by passing review id
        /// </summary>
        /// <param name="reviewId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteReview(string reviewId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/review/{reviewId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a ReviewDTO by passing review id
        /// </summary>
        /// <param name="reviewId">String</param>
        /// <returns>ReviewDTO</returns>
        public async Task<ReviewDTO> GetReviewById(string reviewId)
        {
            ReviewDTO review = new ReviewDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/review/{reviewId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ReviewDTO>();
                    readTask.Wait();

                    review = readTask.Result;
                }
            }

            return review;
        }

        /// <summary>
        /// Creates a review by passing ReviewVO
        /// </summary>
        /// <param name="review">ReviewVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> CreateReview(ReviewVO review)
        {
            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<ReviewVO>("/api/v1/review/create", review);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns reviews by passing an email
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>PageOfReviews</returns>
        public async Task<PageOfReviews> GetAllReviewsByEmail(string email)
        {
            PageOfReviews pageOfReviews = new PageOfReviews();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/review/get-by/{email}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfReviews>();
                    readTask.Wait();

                    pageOfReviews = readTask.Result;
                }
            }

            return pageOfReviews;
        }

        /// <summary>
        /// Updates review by passing ReviewVO
        /// </summary>
        /// <param name="review">ReviewVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateReview(ReviewVO review)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<ReviewVO>($"/api/v1/review/update", review);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }

                return false;
            }
        }

        ////////////////////
        //Schedule Actions//
        ////////////////////

        /// <summary>
        /// Deletes a schedule by passing schedule id
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> DeleteSchedule(string scheduleId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP DELETE
                var deleteTask = client.DeleteAsync($"/api/v1/schedule/{scheduleId}/delete");
                deleteTask.Wait();

                var result = await deleteTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a schedule by passing schedule id 
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <returns>ScheduleDTO</returns>
        public async Task<ScheduleDTO> GetScheduleById(string scheduleId)
        {
            ScheduleDTO schedule = new ScheduleDTO();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/schedule/{scheduleId}/get");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ScheduleDTO>();
                    readTask.Wait();

                    schedule = readTask.Result;
                }
            }

            return schedule;
        }

        /// <summary>
        /// Creates a sechedule by passing a ScheduleVO
        /// </summary>
        /// <param name="schedule">ScheduleVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateSchedule(ScheduleVO schedule)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();

            int statusCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<ScheduleVO>("/api/v1/schedule/create", schedule);
                postTask.Wait();

                var result = await postTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200 || statusCode == 201)
                {
                    var readTask = result.Content.ReadAsAsync<Dictionary<string,string>>();
                    readTask.Wait();

                    response = readTask.Result;
                }
            }
            return response.GetValueOrDefault("scheduleId");
        }

        /// <summary>
        /// Returns all of a sellers schedules as a page by passing an email
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>PageOfSchedules</returns>
        public async Task<PageOfSchedules> GetAllSchedulesByEmail(string email)
        {
            PageOfSchedules schedules = new PageOfSchedules();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/schedule/get-by/{email}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfSchedules>();
                    readTask.Wait();

                    schedules = readTask.Result;
                }
            }

            return schedules;
        }

        /// <summary>
        /// Updates a schedule by passing a ScheduleVO
        /// </summary>
        /// <param name="schedule">ScheduleVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateSchedule(ScheduleVO schedule)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<ScheduleVO>($"/api/v1/schedule/update", schedule);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }

                return false;
            }
        }

        //////////////////
        //Seller Actions//
        //////////////////

        /// <summary>
        /// Returns a list of users based on a location and maximum distance radius
        /// </summary>
        /// <param name="addressId">String</param>
        /// <param name="maxKmDistance">String</param>
        /// <returns>List<UserDTO></returns>
        public async Task<List<UserDTO>> GetGeoFence(string addressId, string maxKmDistance)
        {
            List<UserDTO> users = new List<UserDTO>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/seller/get-geofence?addressId={addressId}&maxKmDistance={maxKmDistance}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<UserDTO>>();
                    readTask.Wait();

                    users = readTask.Result;
                }
            }
            return users;
        }

        ///////////////////
        //Profile Actions//
        ///////////////////

        /// <summary>
        /// Returns the current user, the back end determines this through the token
        /// </summary>
        /// <returns>UserDTO</returns>
        public async Task<UserDTO> GetCurrentUser()
        {
            UserDTO user = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/user/me");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UserDTO>();
                    readTask.Wait();

                    user = readTask.Result;
                }
            }
            return user;
        }

        /// <summary>
        /// Returns the user passing an email
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>UserDTO</returns>
        public async Task<UserDTO> GetUserByEmail(string email)
        {
            UserDTO user = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/user/{email}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UserDTO>();
                    readTask.Wait();

                    user = readTask.Result;
                }
            }
            return user;
        }

        /// <summary>
        /// Edits a user by passing a user object
        /// </summary>
        /// <param name="user">UserDTO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> EditUser(UserVO user)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<UserVO>($"/api/v1/user/updateuser", user);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    var readTask = result.Content.ReadAsAsync<UserDTO>();
                    readTask.Wait();

                    //user = readTask.Result;

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Adds an address to a user by passing an AddressVO object and an email
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="address">AddressVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> AddAddress(string email, AddressVO address)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<AddressVO>($"/api/v1/user/{email}/add-address", address);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Updates an address to a user by passing an AddressVO object and an email
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="address">AddressVO</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> UpdateAddress(string email, AddressVO address)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync<AddressVO>($"/api/v1/user/{email}/update-address", address);
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes an address to a user by passing an Address id object and an email
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="addressId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveAddress(string email, string addressId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/user/{email}/remove-address/{addressId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Reurns a page of items by passing an email, this page of items
        /// represents the users favorites
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="pageNumber">Long</param>
        /// <param name="size">Long</param>
        /// <returns>PageOfItems</returns>
        public async Task<PageOfItems> GetFavoritesByEmail(string email, long pageNumber = 0, long size = 9)
        {
            PageOfItems pageOfItems = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/user/{email}/get-favorites?page={pageNumber}&size={size}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PageOfItems>();
                    readTask.Wait();

                    pageOfItems = readTask.Result;
                }
            }
            return pageOfItems;
        }

        /// <summary>
        /// Adds an item to a users favorites by passing an email and item id
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> AddFavorite(string email, string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/user/{email}/add-favorite/{itemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes an item to a users favorites by passing an email and item id
        /// </summary>
        /// <param name="email">String</param>
        /// <param name="itemId">String</param>
        /// <returns>Bool indicating success or failure</returns>
        public async Task<bool> RemoveFavorite(string email, string itemId)
        {
            int statusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");

                //HTTP PUT
                var putTask = client.PutAsJsonAsync($"/api/v1/user/{email}/remove-favorite/{itemId}", "");
                putTask.Wait();

                var result = await putTask;
                statusCode = (int)result.StatusCode;
                if (statusCode == 200)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns a list of strings, each string should be a country
        /// </summary>
        /// <returns>List<string></returns>
        public async Task<List<string>> GetAllCountries()
        {
            List<string> countries = new List<string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/user/get-countries");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<string>>();
                    readTask.Wait();

                    countries = readTask.Result;
                }
            }

            return countries;
        }

        /// <summary>
        /// Returns a list of StateCountry object based on country
        /// </summary>
        /// <param name="country">String</param>
        /// <returns>List<StateCountry></returns>
        public async Task<List<StateCountry>> FindRegionsByCountry(string country)
        {
            List<StateCountry> regions = new List<StateCountry>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
                //HTTP GET
                var responseTask = client.GetAsync($"/api/v1/user/find-states?country={country}");
                responseTask.Wait();

                var result = await responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<StateCountry>>();
                    readTask.Wait();

                    regions = readTask.Result;
                }
            }

            return regions;
        }








    }
}