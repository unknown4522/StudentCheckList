using Microsoft.AspNetCore.Mvc;
using StudentFilesFrontend.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace StudentFilesFrontend.Services
{
    public class StudentfilesService
    {
        private readonly HttpClient _httpClient;

        public StudentfilesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
       
        public async Task<List<Studentfiles>?> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Studentfiles>>("api/Studentfiles");
        }

        // ✅ GET BY ID (fixed typo: "pi" ➜ "api")
        public async Task<Studentfiles?> GetByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Studentfiles>($"api/Studentfiles/{id}");
        }

        // ✅ ADD STUDENT
        public async Task<HttpResponseMessage> AddAsync(Studentfiles student)
        {
            var queryParams = $"studentID={student.StudentID}&studentname={student.Studentname}&gradelevel={student.Gradelevel}" +
                              $"&remarks={student.Remarks}&status={student.Status}&section={student.Section}" +
                              $"&lrn={student.Lrn}&formerschool={student.Formerschool}&location={student.Location}" +
                              $"&psa={student.PSA}&card={student.Card}&enrollmentform={student.Enrollmentform}" +
                              $"&passportpicture={student.Passportpicture}&academichonors={student.Academichonors}" +
                              $"&indegency={student.Indegency}&form137={student.Form137}";

            return await _httpClient.PostAsync($"api/Studentfiles/add?{queryParams}", null);
        }
        //// ✅ CORRECT
        //public async Task<List<Studentfiles>> GetBySchoolyearAndCampusAsync(string schoolyearName, string campusName)
        //{
        //    try
        //    {
        //        var encodedYear = Uri.EscapeDataString(schoolyearName);
        //        var encodedCampus = Uri.EscapeDataString(campusName);

        //        var response = await _httpClient.GetAsync($"api/Studentfiles?schoolyearName={encodedYear}&campusName={encodedCampus}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var students = await response.Content.ReadFromJsonAsync<List<Studentfiles>>();
        //            return students ?? new List<Studentfiles>();
        //        }

        //        return new List<Studentfiles>();
        //    }
        //    catch
        //    {
        //        return new List<Studentfiles>();
        //    }
        //}
        public async Task<List<Studentfiles>> GetBySchoolyearAndCampusAsync(string schoolyear, string campusName)
        {
            var encodedYear = Uri.EscapeDataString(schoolyear);
            var encodedCampus = Uri.EscapeDataString(campusName);

            var response = await _httpClient.GetAsync($"api/Studentfiles?schoolyearName={encodedYear}&campusName={encodedCampus}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Studentfiles>>() ?? new List<Studentfiles>();
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error: {error}");
        }

    }
}
