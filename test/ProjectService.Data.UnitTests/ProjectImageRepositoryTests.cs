using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
  internal class ProjectImageRepositoryTests
  {
    private IDataProvider _provider;
    private ProjectImageRepository _repository;
    private DbContextOptions<ProjectServiceDbContext> _dbContext;

    private DbProjectImage _image1;
    private DbProjectImage _image2;

    [SetUp]
    public void SetUp()
    {
      CreateImages();

      CreateMemoryDb();

      SaveImages();
    }

    private void CreateImages()
    {
      _image1 = new DbProjectImage()
      {
        Id = Guid.NewGuid(),
        ProjectId = Guid.NewGuid(),
        ImageId = Guid.NewGuid()
      };

      _image2 = new DbProjectImage()
      {
        Id = Guid.NewGuid(),
        ProjectId = Guid.NewGuid(),
        ImageId = Guid.NewGuid()
      };
    }

    public void CreateMemoryDb()
    {
      _dbContext = new DbContextOptionsBuilder<ProjectServiceDbContext>()
        .UseInMemoryDatabase(databaseName: "ProjectServiceTest")
        .Options;

      _provider = new ProjectServiceDbContext(_dbContext);
      _repository = new ProjectImageRepository(_provider);
    }

    private void SaveImages()
    {
      _provider.Images.Add(_image1);
      _provider.Images.Add(_image2);
      _provider.Save();
    }

    #region AddImage

    [Test]
    public async Task ShouldAddImageAsync()
    {
      DbProjectImage image = new DbProjectImage
      {
        Id = Guid.NewGuid(),
        ProjectId = Guid.NewGuid(),
        ImageId = Guid.NewGuid()
      };

      SerializerAssert.AreEqual(new List<Guid> { image.ImageId }, await _repository.CreateAsync(new List<DbProjectImage> { image }));
    }

    [Test]
    public async Task ShouldReturnNullForAddNullImageAsync()
    {
      SerializerAssert.AreEqual(null, await _repository.CreateAsync(null));
    }

    #endregion

    #region RemoveImage

    [Test]
    public async Task ShouldRemoveImageAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.RemoveAsync(new List<Guid> { _image1.ImageId }));
      SerializerAssert.AreEqual(false, _provider.Images.ContainsAsync(_image1).Result);
      SerializerAssert.AreEqual(true, _provider.Images.ContainsAsync(_image2).Result);
    }

    [Test]
    public async Task ShouldReturnNullForRemoveNullImageAsync()
    {
      SerializerAssert.AreEqual(false, await _repository.RemoveAsync(null));
    }

    #endregion

    [TearDown]
    public void CleanDb()
    {
      if (_provider.IsInMemory())
      {
        _provider.EnsureDeleted();
      }
    }
  }
}
