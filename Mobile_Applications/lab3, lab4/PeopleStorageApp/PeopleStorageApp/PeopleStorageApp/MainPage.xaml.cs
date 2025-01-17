﻿using PeopleStorageApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeopleStorageApp
{
    public partial class MainPage : ContentPage
    {
        private Person person = new Person();
        private readonly IPeopleClient client;
        public MainPage(IPeopleClient client)
        {
            InitializeComponent();

            BindingContext = new DataBinding(person);
            this.client = client;
            btnPhoto.Clicked += BtnPhoto_Clicked;
            btnSave.Clicked += BtnSave_Clicked;
            //tbxFirstName.TextChanged += TbxFirstName_TextChanged;
            //tbxLastName.TextChanged += TbxLastName_TextChanged;
            //tbxPhoneNumber.TextChanged += TbxPhoneNumber_TextChanged;

        }
        /*
        private void TbxPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            person.PhoneNumber = e.NewTextValue;
        }

        private void TbxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            person.LastName = e.NewTextValue;
        }

        private void TbxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            person.FirstName = e.NewTextValue;
        }
        */
        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (!Validate())
            {
                await DisplayAlert("Validation error", "First name, last name, phone number and picture are required.", "Ok");
                return;
            }

            try
            {
                await client.AddPersonAsync(person);
                await DisplayAlert("Success", "Data has been saved", "Ok");
                Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void Clear()
        {
            //tbxFirstName.Text = string.Empty;
            //tbxLastName.Text = string.Empty;
            //tbxPhoneNumber.Text = string.Empty;
            imgPhoto.Source = null;
            person = new Person();
            

        }

        private async void BtnPhoto_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions { });
            if (photo == null)
            {
                return;
            }

            imgPhoto.Source = ImageSource.FromStream(() => photo.GetStream());
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                photo.GetStream().CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            person.PictureBase64 = base64;
        }

        private bool Validate()
        {
            return !(
                string.IsNullOrWhiteSpace(person.FirstName) ||
                string.IsNullOrWhiteSpace(person.LastName) ||
                string.IsNullOrWhiteSpace(person.PhoneNumber) ||
                string.IsNullOrWhiteSpace(person.PictureBase64)
                );
        }
    }
}
