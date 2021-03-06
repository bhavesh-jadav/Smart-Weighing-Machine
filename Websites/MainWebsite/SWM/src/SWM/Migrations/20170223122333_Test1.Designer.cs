﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SWM.Models;

namespace SWM.Migrations
{
    [DbContext(typeof(SwmContext))]
    [Migration("20170223122333_Test1")]
    partial class Test1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SWM.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("SWM.Models.CropData", b =>
                {
                    b.Property<int>("ProductToUserId");

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("UserLocationToMachineId");

                    b.Property<int>("Weight");

                    b.HasKey("ProductToUserId", "DateTime");

                    b.HasIndex("ProductToUserId");

                    b.HasIndex("UserLocationToMachineId");

                    b.ToTable("CropDatas");
                });

            modelBuilder.Entity("SWM.Models.MachineInformation", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAssigned");

                    b.Property<DateTime>("ManufactureDate");

                    b.Property<string>("ManufactureLocation")
                        .IsRequired();

                    b.Property<DateTime>("SellDate");

                    b.HasKey("MachineId");

                    b.ToTable("MachineInformations");
                });

            modelBuilder.Entity("SWM.Models.MachineToUser", b =>
                {
                    b.Property<int>("MachineId");

                    b.Property<string>("UserID");

                    b.HasKey("MachineId", "UserID");

                    b.HasIndex("MachineId");

                    b.HasIndex("UserID");

                    b.ToTable("MachineToUsers");
                });

            modelBuilder.Entity("SWM.Models.OtherData", b =>
                {
                    b.Property<string>("Name");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Name");

                    b.ToTable("OtherDatas");
                });

            modelBuilder.Entity("SWM.Models.ProductInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ProductInformations");
                });

            modelBuilder.Entity("SWM.Models.ProductsToUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProductId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductsToUsers");
                });

            modelBuilder.Entity("SWM.Models.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("SWM.Models.SubscriptionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("SubscriptionTypes");
                });

            modelBuilder.Entity("SWM.Models.SwmUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("CountryId");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("PinNo");

                    b.Property<DateTime>("RegisterDate");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("StateId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("StateId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SWM.Models.UserLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<int>("CountryId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PinNo");

                    b.Property<int>("StateId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("StateId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLocations");
                });

            modelBuilder.Entity("SWM.Models.UserLocationToMachine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MachineId");

                    b.Property<int>("UserLocationId");

                    b.HasKey("Id");

                    b.HasIndex("MachineId");

                    b.HasIndex("UserLocationId");

                    b.ToTable("UserLocationToMachines");
                });

            modelBuilder.Entity("SWM.Models.UserToSubscription", b =>
                {
                    b.Property<string>("UserID");

                    b.Property<string>("SubscriptionId");

                    b.Property<int>("SubscriptionTypeId");

                    b.HasKey("UserID", "SubscriptionId");

                    b.HasIndex("SubscriptionTypeId");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("UserToSubscriptions");
                });

            modelBuilder.Entity("SWM.Models.UserRoleManager", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole");


                    b.ToTable("UserRoleManager");

                    b.HasDiscriminator().HasValue("UserRoleManager");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SWM.Models.SwmUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SWM.Models.SwmUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.SwmUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.CropData", b =>
                {
                    b.HasOne("SWM.Models.ProductsToUser", "ProductsToUser")
                        .WithMany("CropData")
                        .HasForeignKey("ProductToUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.UserLocationToMachine", "UserLocationToMachine")
                        .WithMany("CropData")
                        .HasForeignKey("UserLocationToMachineId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.MachineToUser", b =>
                {
                    b.HasOne("SWM.Models.MachineInformation", "MachineInformation")
                        .WithMany("MachineToUser")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.SwmUser", "SwmUser")
                        .WithMany("MachineToUser")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.ProductsToUser", b =>
                {
                    b.HasOne("SWM.Models.ProductInformation", "ProductInformation")
                        .WithMany("ProductsToUser")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.SwmUser", "SwmUser")
                        .WithMany("ProductToUser")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.SwmUser", b =>
                {
                    b.HasOne("SWM.Models.Country", "Country")
                        .WithMany("SwmUser")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.State", "State")
                        .WithMany("SwmUser")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.UserLocation", b =>
                {
                    b.HasOne("SWM.Models.Country", "Country")
                        .WithMany("UserLocation")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.State", "State")
                        .WithMany("UserLocation")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.SwmUser", "SwmUser")
                        .WithMany("UserLocation")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.UserLocationToMachine", b =>
                {
                    b.HasOne("SWM.Models.MachineInformation", "MachineInformation")
                        .WithMany("UserLocationToMachine")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.UserLocation", "UserLocation")
                        .WithMany("UserLocationToMachine")
                        .HasForeignKey("UserLocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SWM.Models.UserToSubscription", b =>
                {
                    b.HasOne("SWM.Models.SubscriptionType", "SubscriptionType")
                        .WithMany("UserToSubscription")
                        .HasForeignKey("SubscriptionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SWM.Models.SwmUser", "SwmUser")
                        .WithOne("UserToSubscription")
                        .HasForeignKey("SWM.Models.UserToSubscription", "UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
