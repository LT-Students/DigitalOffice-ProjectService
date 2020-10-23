﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef.Database.Migrations
{
    [DbContext(typeof(ProjectServiceDbContext))]
    [Migration("20200802205013_PublicBoolProperties")]
    partial class PublicBoolProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Database.Entities.DbProject", b =>
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

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Database.Entities.DbProjectFile", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProjectId", "FileId");

                    b.ToTable("ProjectFile");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Database.Entities.DbProjectWorkerUser", b =>
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

                    b.ToTable("ProjectWorkerUser");
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Database.Entities.DbProjectFile", b =>
                {
                    b.HasOne("LT.DigitalOffice.ProjectService.Database.Entities.DbProject", "Projects")
                        .WithMany("FilesIds")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LT.DigitalOffice.ProjectService.Database.Entities.DbProjectWorkerUser", b =>
                {
                    b.HasOne("LT.DigitalOffice.ProjectService.Database.Entities.DbProject", "Projects")
                        .WithMany("WorkersUsersIds")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
