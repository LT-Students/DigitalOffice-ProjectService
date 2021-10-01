using LT.DigitalOffice.ProjectService.Mappers.Db;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    public class PatchDbProjectMapperTests
    {
        private IPatchDbProjectMapper _mapper;
        private JsonPatchDocument<EditProjectRequest> _request;
        private JsonPatchDocument<DbProject> _dbRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            #region requsest data
            _request = new JsonPatchDocument<EditProjectRequest>(new List<Operation<EditProjectRequest>>
            {
                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Name)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.ShortName)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Description)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.ShortDescription)}",
                    "",
                    "value"),

                new Operation<EditProjectRequest>(
                    "replace",
                    $"/{nameof(EditProjectRequest.Status)}",
                    "",
                    ProjectStatusType.Active),

            }, new CamelCasePropertyNamesContractResolver());
            #endregion

            #region response data
            _dbRequest = new JsonPatchDocument<DbProject>(new List<Operation<DbProject>>
            {
                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Name)}",
                    "",
                    "value"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.ShortName)}",
                    "",
                    "value"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Description)}",
                    "",
                    "value"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.ShortDescription)}",
                    "",
                    "value"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Status)}",
                    "",
                    ProjectStatusType.Active)

            }, new CamelCasePropertyNamesContractResolver());
            #endregion

            _mapper = new PatchDbProjectMapper();
        }

        [Test]
        public void SuccessMap()
        {
            SerializerAssert.AreEqual(_dbRequest, _mapper.Map(_request));
        }

        [Test]
        public void ThrowNullArqumentException()
        {
            JsonPatchDocument<EditProjectRequest> nullRequest = null;
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(nullRequest));
        }
    }
}
