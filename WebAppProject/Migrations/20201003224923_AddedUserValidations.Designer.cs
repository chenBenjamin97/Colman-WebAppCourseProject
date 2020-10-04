﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAppProject.Data;

namespace WebAppProject.Migrations
{
    [DbContext(typeof(MvcProjectContext))]
    [Migration("20201003224923_AddedUserValidations")]
    partial class AddedUserValidations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebAppProject.Models.ContactApplication", b =>
                {
                    b.Property<int>("ContactAppID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContactType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ContactAppID");

                    b.HasIndex("UserID");

                    b.ToTable("ContactApplication");
                });

            modelBuilder.Entity("WebAppProject.Models.ElectricityTransaction", b =>
                {
                    b.Property<int>("ElectricityMeterID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ElectricityMeterLastRead")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ElectricityMeterID");

                    b.HasIndex("UserID");

                    b.ToTable("ElectricityTransactions");
                });

            modelBuilder.Entity("WebAppProject.Models.PropertyTaxTransaction", b =>
                {
                    b.Property<int>("PropertyID")
                        .HasColumnType("int");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("PropertyID");

                    b.HasIndex("UserID");

                    b.ToTable("PropertyTaxTransactions");
                });

            modelBuilder.Entity("WebAppProject.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int?>("ApartmentNumber")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EnteranceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropertyCity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropertyStreet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropertyStreetNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("WebAppProject.Models.ViewModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("ID");

                    b.ToTable("ViewModel");
                });

            modelBuilder.Entity("WebAppProject.Models.WaterTransaction", b =>
                {
                    b.Property<int>("WaterMeterID")
                        .HasColumnType("int");

                    b.Property<string>("ImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("WaterMeterLastReadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("WaterMeterID");

                    b.HasIndex("UserID");

                    b.ToTable("WaterTransactions");
                });

            modelBuilder.Entity("WebAppProject.Models.ContactApplication", b =>
                {
                    b.HasOne("WebAppProject.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebAppProject.Models.ElectricityTransaction", b =>
                {
                    b.HasOne("WebAppProject.Models.User", "User")
                        .WithMany("ElectricityTransactions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebAppProject.Models.PropertyTaxTransaction", b =>
                {
                    b.HasOne("WebAppProject.Models.User", "User")
                        .WithMany("PropertyTaxTransactions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebAppProject.Models.WaterTransaction", b =>
                {
                    b.HasOne("WebAppProject.Models.User", "User")
                        .WithMany("WaterTransactions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
