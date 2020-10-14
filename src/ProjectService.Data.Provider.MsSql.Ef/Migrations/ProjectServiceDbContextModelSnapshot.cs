﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    partial class ProjectServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Deferred")
                        .HasColumnType("bit");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProjectFile", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProjectId", "FileId");

                    b.ToTable("DbProjectFile");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProjectWorkerUser", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WorkerUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AddedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsManager")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("RemovedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("ProjectId", "WorkerUserId");

                    b.ToTable("DbProjectWorkerUser");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProjectFile", b =>
                {
                    b.HasOne("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProject", "Project")
                        .WithMany("FilesIds")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProjectWorkerUser", b =>
                {
                    b.HasOne("LT.DigitalOffice.ProjectService.Models.Db.Entities.DbProject", "Project")
                        .WithMany("WorkersUsersIds")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
