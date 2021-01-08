var $$ = Dom7;
//var APIUrl = "http://localhost:56750/";
var APIUrl = "https://nff.construcodeapp.com/";
var isMyNudesLoading = false;
var isMyNudesFullyLoaded = false;
var isInteractionsLoading = false;
var isInteractionsFullyLoaded = false;
var isPostCommentsFullyLoaded = false;
var isPostCommentsLoading = false;
// Loading flag
var allowInfinite = true;
var lastID = [0, 0, 0, 0, 0];
var lockCategory = [0, 0, 0, 0, 0];
var currentCategory = 0;
var totalToGet = 10;
var lastPostLength = 10;
adjustFooterActive = (screen) => {
  $.each($(".footer-tab-button"), i => $($(".footer-tab-button")[i]).removeClass("tab-link-active"));
  $('.' + screen).addClass("tab-link-active");
}

// App configuration
var app = new Framework7({
  root: '#app',
  theme: 'ios',
  tapHold: true,
  view: {
    stackPages: false,
  },
  dialog: {
    title: 'NudesForFree',
    buttonOk: 'Ok',
    buttonCancel: 'Cancelar'
  },
  // Create routes for all pages
  routes: [
    {
      path: '/',
      url: 'index.html',
      on: {
        pageBeforeIn: () => { currentCategory = 0; lastID = [0, 0, 0, 0, 0]; allowInfinite = true; loadNudes(); },
        pageInit: () => adjustFooterActive("index-page")
      }
    },
    {
      path: '/home/',
      url: 'index.html',
      on: {
        pageBeforeIn: () => {
          currentCategory = 0; lastID = [0, 0, 0, 0, 0]; allowInfinite = true; loadNudes();
          $('.infinite-scroll-content').on('infinite', function () {
            scrollPosts();
          });
        },
        pageInit: () => adjustFooterActive("index-page")
      }
    },
    {
      path: '/user/',
      url: 'pages/profile.html',
      on: {
        pageBeforeIn: () => loadProfileData(),
        pageInit: () => adjustFooterActive("user-page")
      }
    },
    {
      path: '/interactions/',
      url: 'pages/interactions.html',
      on: {
        pageBeforeIn: () => loadInteractions(false),
        pageInit: (e, page) => {
          adjustFooterActive("interactions-page");
          isInteractionsFullyLoaded = isInteractionsLoading = false;
          page.$el.find('#interactions-infinity').on('infinite', function () {
            if (isInteractionsLoading || isInteractionsFullyLoaded) return;
            isInteractionsLoading = true;
            loadInteractions(true);
          });
        }
      }
    },
    {
      path: '/my-nudes/',
      url: 'pages/my-nudes.html',
      on: {
        pageBeforeIn: () => loadMyNudes(false),
        pageInit: (e, page) => {
          adjustFooterActive("my-nudes-page");
          isMyNudesFullyLoaded = isMyNudesLoading = false;
          page.$el.find('#my-nudes-infinity').on('infinite', function () {
            if (isMyNudesLoading || isMyNudesFullyLoaded) return;
            isMyNudesLoading = true;
            loadMyNudes(true);
          });
        }
      }
    },
    {
      path: '/post/:postID/:filename/:description/:username/:likes/:category/:avatar/:postDate',
      url: 'pages/post.html',
      on: {
        pageInit: (e, page) => {
          const { postID, filename, description, username, likes, category, avatar, postDate } = page.route.params;
          $("#postID").text(postID);
          $("#post-image").attr("src", APIUrl + "posts/" + filename);
          if (avatar)
            $("#post_user_avatar").attr("src", APIUrl + "avatars/" + avatar);
          $("#post_username").text(username);
          $("#total_likes").html(likes);
          $("#post_description").text(description);
          $("#post_date").text(formatDate(postDate));

          isPostCommentsFullyLoaded = isPostCommentsLoading = false;
          page.$el.find('#post-comment-infinity').on('infinite', function () {
            if (isPostCommentsLoading || isPostCommentsFullyLoaded) return;
            isPostCommentsLoading = true;
            loadPostComments(postID);
          });
          loadPostComments(postID, true);
        }
      }
    },
    {
      path: '/post/:postID',
      url: 'pages/post.html',
      on: {
        pageInit: (e, page) => {
          const { postID } = page.route.params;
          $("#postID").text(postID);
          loadPost(postID);
        }
      }
    },
    {
      path: '/camera/',
      url: 'pages/camera.html',
      on: {
        pageInit: () => adjustFooterActive("camera-page")
      }
    },
    {
      path: '/account-create/',
      url: 'pages/account-create.html',
    },
    {
      path: '/account-photo/:edit',
      url: 'pages/account-photo.html',
      on: {
        pageInit: (e, page) => {
          const { edit } = page.route.params;
          if (edit == 1) {
            $("#account-photo-title").html("Trocar foto");
          }
        }
      }
    },
    {
      path: '/login/',
      url: 'pages/login.html',
    }
  ]
});

// Create the tabs views
var mainView = app.views.create('.view-main');


incEffect = (newValue, obj) => {
  let currentValue = parseInt(obj.text());
  if (currentValue < newValue) {
    obj.text(currentValue + 1);
    let delay = newValue - currentValue >= 50 ? 10 : 20;
    setTimeout(() => incEffect(newValue, obj), delay);
  }
}

editPhoto = () => {
  app.router.navigate('/account-photo/1');
}

openDeleteAccountModal = () => {
  app.dialog.confirm('Essa ação apagará de maneira irreversível toda a sua história no NudesForFree. ' +
    'Tem certeza que quer fazer isso?', function () {
      $.post(APIUrl + "User/Delete",
        {
          token: Cookies.get("nffTkn")
        },
        (result) => {
          if (result.success) {
            logOut();
            app.router.navigate("/");
            app.toast.create({
              text: '=(',
              position: 'top',
              closeTimeout: 2000,
            }).open();
            return;
          }
          app.toast.create({
            text: 'Tivemos um problema. Tente novamente.',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        })
        .fail((e) => {
          app.toast.create({
            text: 'Tivemos um problema. Tente novamente.',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        });
    });
}

loadProfileData = (onlyCookieUpdate) => {
  if (!onlyCookieUpdate && Cookies.get("nff_profile")) {
    let cookieProfileData = JSON.parse(Cookies.get("nff_profile"));
    $("#profile_avatar").attr("src", APIUrl + "/avatars/" + cookieProfileData.avatar);
    $("#profile_username").append("@" + cookieProfileData.username);
    $("#profile_nudeCount").append(cookieProfileData.postCount);
    $("#profile_commentCount").append(cookieProfileData.commentCount);
    $("#profile_likeCount").append(cookieProfileData.likeCount);
  }

  $.post(APIUrl + "User/GetProfileData",
    {
      userToken: Cookies.get("nffTkn")
    },
    (result) => {
      if (result.success) {
        let profileData = result.data;
        Cookies.set("nff_profile", {
          avatar: profileData.avatar,
          username: profileData.username,
          postCount: profileData.postCount,
          commentCount: profileData.commentCount,
          likeCount: profileData.likeCount
        });
        if (onlyCookieUpdate)
          return;
        $("#profile_avatar").attr("src", APIUrl + "/avatars/" + profileData.avatar);
        $("#profile_username").text("@" + profileData.username);

        if ($("#profile_nudeCount").text()) {
          incEffect(profileData.postCount, $("#profile_nudeCount"));
          incEffect(profileData.commentCount, $("#profile_commentCount"));
          incEffect(profileData.likeCount, $("#profile_likeCount"));
        } else {
          $("#profile_nudeCount").text(profileData.postCount);
          $("#profile_commentCount").text(profileData.commentCount);
          $("#profile_likeCount").text(profileData.likeCount);
        }

        $('#profile_bestNudes').empty();
        if (profileData.bestPosts.length == 0)
          $('#noBestNudes').show();
        profileData.bestPosts.forEach(post => {
          $(mountBestNudesItem(post)).appendTo('#profile_bestNudes');
        });
        return;
      }

      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
      logOut();
    })
    .fail((e) => {
      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
      logOut();
    });
}

loadInteractions = (toInfinity) => {
  let lastID = 0;
  if (toInfinity && $("#interactions-infinity .skeletonFade").length == 0) {
    let lastPostItem = $($("#interactions-infinity a")[$("#interactions-infinity a").length - 1]);
    lastID = parseInt(lastPostItem.attr("id").split("interaction_")[1]);
  }

  $.post(APIUrl + "Interaction/GetInteractions",
    {
      token: Cookies.get("nffTkn"),
      lastID: lastID
    },
    (result) => {
      if (result.success) {
        if (!toInfinity) {
          $("#interactions-infinity a").remove();
          if (result.data.length == 0) {
            $("#noInteraction").show();
            $("#interactions-infinity").remove();
            return;
          }
        }
        $("#interactions-infinity .preloader").remove();
        result.data.forEach(post => {
          $(mountMyInteractions(post)).appendTo('#interactions-infinity');
        });
        if (result.data.length == 10)
          $("#interactions-infinity").append("<div class='preloader infinite-scroll-preloader'></div>");
        if (toInfinity) {
          isInteractionsLoading = false;
          if (result.data.length == 0) {
            isInteractionsFullyLoaded = true;
            $(".infinite-content-div .preloader").remove();
          }
        }
        return;
      }
      if (!toInfinity)
        app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    })
    .fail((e) => {
      if (!toInfinity)
        app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
}

loadMyNudes = (toInfinity) => {
  let lastID = 0;
  if (toInfinity) {
    let lastPostItem = $($(".post-list li")[$(".post-list li").length - 1]);
    lastID = parseInt(lastPostItem.attr("id").split("myNudesPost_")[1]);
  }

  $.post(APIUrl + "User/GetUserPosts",
    {
      token: Cookies.get("nffTkn"),
      lastID: lastID,
      count: 10
    },
    (result) => {
      if (result.success) {
        if (!toInfinity) {
          $("#my-nudes-infinity .list").empty();
          if (result.data.length == 0) {
            $("#noNude").show();
            $("#my-nudes-infinity").hide();
            return;
          }
        }
        result.data.forEach(post => {
          $(mountMyNudesItem(post)).appendTo('#my-nudes-infinity .list');
        });
        if (result.data.length < 10)
          $("#my-nudes-infinity .preloader").remove();
        if (toInfinity) {
          isMyNudesLoading = false;
          if (result.data.length == 0) {
            isMyNudesFullyLoaded = true;
            $("#my-nudes-infinity .preloader").remove();
          }
        }
        return;
      }
      if (!toInfinity)
        app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    })
    .fail((e) => {
      if (!toInfinity)
        app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
}

loadNudes = () => {
  if (lockCategory[currentCategory] != 0)
    return;
  lockCategory[currentCategory] = 1;
  $.post(APIUrl + "Post/GetPosts",
    {
      lastID: lastID[currentCategory],
      count: totalToGet,
      category: currentCategory,
      token: Cookies.get("nffTkn")
    },
    (result) => {
      if (result.success) {
        var tmpCategory = currentCategory;
        if (result.data.length > 0) {
          currentCategory = result.data[0].category;
        }
        $("#" + getCurrentCategory() + " .skeletonFade").remove();
        lastPostLength = result.data.length;

        result.data.forEach(post => {
          currentCategory = post.category;
          $(mountPostItem(post)).appendTo('#' + getCurrentCategory() + " .block");

          // $("#like" + post.postID).click(function(e) {
          //   e.stopImmediatePropagation();      
          //   e.stopPropagation();
          //   let tempHref = $("#postArchor_"+post.postID).attr("href");
          //   $("#postArchor_"+post.postID).attr("href","#");
          //   setTimeout(() => $("#postArchor_"+post.postID).attr("href",tempHref), 500);
          //   likePost(post.postID); 
          // });

          if (lastID[currentCategory] == 0 || lastID[currentCategory] > post.postID)
            lastID[currentCategory] = post.postID;
        });
        currentCategory = tmpCategory;
        lockCategory[currentCategory] = 0;
        return;
      }
      lockCategory[currentCategory] = 0;
      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    })
    .fail((e) => {
      lockCategory[currentCategory] = 0;
      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
}
showPreLoader = () => {
  app.preloader.show();
  setTimeout(function () { app.preloader.hide(); }, 5000);
}

sendNude = async () => {
  if ($('#takenImage').length > 0) {
    app.preloader.show();

    var imgCanvas = document.getElementById('takenImage');
    imgCanvas.toBlob(imgBlob => {
      let description = $('#photoDescription').val();
      let category = 0;

      switch ($("#post_category_selector .item-after").text()) {
        case "Feminino":
          category = 0;
          break;
        case "Masculino":
          category = 1;
          break;
        case "HxM (Homem com Mulher)":
          category = 2;
          break;
        case "MxM (Mulher com Mulher)":
          category = 3;
          break;
        case "HxH (Homem com Homem)":
          category = 4;
          break;
      }

      var fd = new FormData();
      fd.append('postImage', imgBlob);
      fd.append('userToken', Cookies.get("nffTkn"));
      fd.append('description', description);
      fd.append('category', category);
      $.ajax({
        type: 'POST',
        url: APIUrl + "Post/CreatePost",
        data: fd,
        processData: false,
        contentType: false,
        success: (data) => {
          app.preloader.hide();
          if (data.success) {
            app.toast.create({
              text: 'Foto enviada!',
              position: 'top',
              closeTimeout: 2000,
            }).open();
            navigateToIndex();
            return;
          }
          app.toast.create({
            text: 'Ocorreu um erro ao enviar a foto. Tente novamente',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        },
        error: (data) => {
          app.preloader.hide();
          app.toast.create({
            text: 'Ocorreu um erro ao enviar a foto. Tente novamente',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        }
      });

    }, 'image/jpeg', 0.5);

  } else {
    app.toast.create({
      text: 'Tire ou selecione um nude para enviar',
      position: 'top',
      closeTimeout: 2000,
    }).open();
  }
};

uploadFile = (elm, crop) => {
  let props;
  if (crop) {
    props = {
      noRevoke: true,
      canvas: false,
      contain: true
    };
  } else {
    props = {
      noRevoke: true,
      canvas: false,
      contain: true,
      orientation: true
    };
  }
  var loadingImage = loadImage(
    elm.files[0],
    function (img) {
      $("#upload-image-background").hide();
      $("#photo-upload-btn").hide();
      $(img).addClass("card-image");
      $(img).attr('id', 'takenImage');
      $(img).click(() => {
        $(".image-cover-dialog").show();
        $(".image-cover-dialog").css("display", "flex");
      });
      document.getElementById("card-image-container").appendChild(img);
      if (crop) {
        $('#takenImage').croppie({
          viewport: {
            width: 120,
            height: 120,
            type: 'circle'
          },
          enableExif: true
        });
        $("#profilePhotoChangeLink").show();
        $("#accountProfilePhotoSubmitBtn").removeClass("disabled");
      }
    },
    props
  );
};

sendAccountPhoto = () => {
  if ($('#takenImage').length > 0) {
    app.preloader.show();
    $('#takenImage').croppie('result', { type: "blob" }).then(croppedImg => {
      var fd = new FormData();
      fd.append('avatar', croppedImg);
      fd.append('userToken', Cookies.get("nffTkn"));
      $.ajax({
        type: 'POST',
        url: APIUrl + "User/UploadUserAvatar",
        data: fd,
        processData: false,
        contentType: false,
        success: (response) => {
          app.preloader.hide();
          if (response.success) {
            let profileCookieData = JSON.parse(Cookies.get("nff_profile"));
            profileCookieData.avatar = response.data;
            Cookies.set("nff_profile", profileCookieData);
            app.toast.create({
              text: 'Tudo pronto! MANDA NUDES!',
              position: 'top',
              closeTimeout: 2000,
            }).open();
            navigateToIndex();
            return;
          }
          app.toast.create({
            text: 'Ocorreu um erro ao enviar a foto. Tente novamente',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        },
        error: (data) => {
          app.preloader.hide();
          app.toast.create({
            text: 'Ocorreu um erro ao enviar a foto. Tente novamente',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        }
      });
    });
  }
};

seePhoto = (imageID) => {
  let can = document.getElementById(imageID);
  var myPhotoBrowserDark = app.photoBrowser.create({
    photos: [can.toDataURL()],
    theme: 'dark'
  });
  myPhotoBrowserDark.open();
};

takeAnother = () => {
  deletePhoto();
  $("#accountProfilePhotoSubmitBtn").addClass("disabled");
  document.querySelector('#photo-input').click();
};

deletePhoto = () => {
  $('#takenImage').croppie('destroy');
  $('#takenImage').remove();
  $(".image-cover-dialog").hide();
  $("#upload-image-background").show();
  $("#photo-upload-btn").show();
};

accountCreationCheckboxChange = () => {
  if ($("#useTermsChk").is(":checked"))
    $("#accountCreationSubmitBtn").removeClass("disabled");
  else
    $("#accountCreationSubmitBtn").addClass("disabled");
};

createAccount = () => {
  if ($("#useTermsChk").is(":checked")) {
    let password = $("#password").val();
    let passwordRetype = $("#passwordRetype").val();
    if (password !== passwordRetype) {
      app.toast.create({
        text: 'As senhas não correspondem',
        position: 'top',
        closeTimeout: 2000,
      }).open();
      return;
    }
    let email = $("#email").val();
    let login = $("#login").val();
    if (!isEmailValid(email)) {
      app.toast.create({
        text: 'Insira um email válido',
        position: 'top',
        closeTimeout: 2000,
      }).open();
      return;
    }

    showPreLoader();
    $.post(APIUrl + "User/Create",
      {
        Username: login,
        Password: password,
        EmailAddress: email,
        Country: 1,
        PreferredCategory: 0
      },
      (data) => {
        app.preloader.hide();
        if (data.success) {
          Cookies.set('nffTkn', data.data, { expires: 365 });
          Cookies.set("nff_profile", {
            avatar: "default_avatar.png",
            username: login,
            postCount: 0,
            commentCount: 0,
            likeCount: 0
          });
          app.router.navigate('/account-photo/0');
          return;
        }
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      })
      .fail((e) => {
        app.preloader.hide();
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      });
  }
};

isEmailValid = (email) => {
  var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return re.test(String(email).toLowerCase());
};

login = () => {
  let username = $('#usernameLogin').val();
  let password = $('#passwordLogin').val();
  if (!username) {
    app.toast.create({
      text: 'Informe um login',
      position: 'top',
      closeTimeout: 2000,
    }).open();
    return;
  }
  if (!password) {
    app.toast.create({
      text: 'Informe a senha',
      position: 'top',
      closeTimeout: 2000,
    }).open();
    return;
  }

  showPreLoader();
  $.post(APIUrl + "User/LoginUser",
    {
      Username: username,
      Password: password
    },
    (data) => {
      app.preloader.hide();
      if (data.success) {
        Cookies.set('nffTkn', data.data, { expires: 365 });
        loadProfileData(data.data);
        navigateToIndex();
        app.toast.create({
          text: 'Login realizado com sucesso!',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      } else {
        app.toast.create({
          text: 'Email e/ou senha não correspondem!',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      }
    })
    .fail((e) => {
      app.preloader.hide();
      app.toast.create({
        text: 'Ocorreu um erro na operação. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });


};

logOut = () => {
  Cookies.remove("nffTkn");
  Cookies.remove("nff_profile");
  app.toast.create({
    text: 'Usuário deslogado',
    position: 'top',
    closeTimeout: 2000,
  }).open();
  navigateToIndex();
};

mountBestNudesItem = (post) => {
  let href = "/post/" +
    post.postID + "/" +
    post.filename + "/" +
    (post.description ? post.description : " ") + "/" +
    post.username + "/" +
    post.likes + "/" +
    post.category + "/";

  var userProfile = JSON.parse(Cookies.get("nff_profile"));
  href += userProfile.avatar;
  href += "/" + post.creationDate;
  return "<li>" +
    "          <a href='" + href + "'>" +
    "            <div class='item-content'>" +
    "              <div class='item-media'><img src='" + APIUrl + "/posts/" + post.filename + "'></div>" +
    "              <div class='item-inner'>" +
    "                <div class='item-subtitle'>" + getCategoryName(post.category) + "</div>" +
    "                <div class='item-title'>" + post.description + "</div>" +
    "                <div class='item-subtitle bottom-subtitle'><i class='icon ion-md-time'></i>" + formatDate(post.creationDate) + "</div>" +
    "              </div>" +
    "            </div>" +
    "          </a>" +
    "        </li>";
}

mountMyInteractions = (interaction) => {
  let href = "/post/" +
    interaction.postID + "/" +
    interaction.filename + "/" +
    (interaction.description ? interaction.description : " ") + "/" +
    interaction.username + "/" +
    interaction.likes + "/" +
    interaction.category + "/" +
    interaction.avatar + "/" +
    interaction.postDate + "/";

  return "<a href='" + href + "' id='interaction_" + interaction.interactionID + "'>" +
    "          <div class='card'>" +
    "            <img class='card-image' src='" + APIUrl + "/posts/" + interaction.filename + "' alt=''>" +
    "            <div class='card-infos'>" +
    "                <div class='card-author custom-card-header'>" +
    "                  <img class='card-author-image' src='" + APIUrl + "/avatars/" + interaction.avatar + "' alt=''>" +
    "                  <div>" + interaction.username + "</div>" +
    "                </div>" +
    "              <div class='card-bottom' style='float: right;'>" +
    "                <div class='card-comments'>" +
    "                  <i class='icon ion-ios-heart'></i>" + interaction.likes +
    "                  <i class='icon ion-ios-text' style='margin-left: 10px;'></i>" + interaction.comments +
    "                </div>" +
    "              </div>" +
    "            </div>" +
    "          </div>" +
    "        </a>";
};

mountMyNudesItem = (post) => {
  let href = "/post/" +
    post.postID + "/" +
    post.filename + "/" +
    (post.description ? post.description : " ") + "/" +
    post.username + "/" +
    post.likes + "/" +
    post.category + "/";

  var userProfile = JSON.parse(Cookies.get("nff_profile"));
  href += userProfile.avatar;
  href += "/" + post.creationDate;

  return "<li class='swipeout' id='myNudesPost_" + post.postID + "'>" +
    "          <a href='" + href + "'>" +
    "            <div class='item-content swipeout-content'>" +
    "              <div class='item-media'><img src='" + APIUrl + "/Posts/" + post.filename + "' alt=''></div>" +
    "              <div class='item-inner'>" +
    "                <div class='item-subtitle'>" + getCategoryName(post.category) +
    "                </div>" +
    "                <div class='item-title'>" + post.description + "</div>" +
    "                <div class='item-subtitle bottom-subtitle'>" +
    "                  <i class='icon ion-md-time'></i>" + formatDate(post.creationDate) +
    "                </div>" +
    "                <div class='item-subtitle bottom-subtitle'>" +
    "                  <i class='icon ion-ios-heart'></i>" + post.likes +
    "                  <i class='icon ion-ios-text' style='margin-left: 10px;'></i>" + post.comments +
    "              </div>" +
    "              </div>" +
    "            </div>" +
    "          </a>" +
    "          <div class='swipeout-actions-right'>" +
    "            <a onClick='deletePost(" + post.postID + ")' class='color-red'>Apagar</a>" +
    "          </div>" +
    "        </li>"
}

mountPostItem = (post) => {
  let href = "/post/" +
    post.postID + "/" +
    post.filename + "/" +
    (post.description ? post.description : " ") + "/" +
    post.username + "/" +
    post.likes + "/" +
    post.category + "/" +
    post.creatorAvatar + "/" +
    post.creationDate;

  return '<div  class="post" id="pstID' + post.postID + '"> ' +
    ' <div class="title-container">' +
    '  <span class="title-date">' + formatDate(post.creationDate) + '</span>' +
    ' </div>' +
    ' <a id="postArchor_' + post.postID + '" href="' + href + '" ondblclick="likePost(' + post.postID + ');">' +
    '   <div class="card">' +
    '     <img class="card-image" src="' + APIUrl + 'posts/' + post.filename + '" alt="">' +
    '     <div class="card-infos">' +
    (post.Likes > 10 ? '       <div class="chip color-red shakingIcon " ><i class="icon ion-ios-flame"></i>Hot</div>' : '') +
    '       <div class="card-bottom">' +
    '         <div class="card-author">' +
    '           <img class="card-author-image"  src="' + APIUrl + 'avatars/' + post.creatorAvatar + '"  alt="">' +
    '           <div>' + post.username + '</div>' +
    '         </div>' +
    '         <div class="card-comments">' +
    '           <i id="like' + post.postID + '" class="icon ion-ios-heart' + (post.hasLike ? "" : "-empty") + '" style="margin-top: -4%;"></i><span id="countLike' + post.postID + '">' + post.likes + '</span>' +
    '           <i class="icon ion-ios-text" style="margin-top: -2px;margin-left: 12px;"></i>' + post.comments +
    '         </div>' +
    '       </div>' +
    '     </div>' +
    '   </div>' +
    ' </a>' +
    '</div>';
}
mountCommentItem = (comment) => {
  return ' <li id ="post_comment_' + comment.commentID + '"> ' +
    '    <a href="#" class="item item-content"> ' +
    '      <div class="item-media"><img src="' + APIUrl + '/avatars/' + comment.avatar + '" alt=""></div> ' +
    '      <div class="item-inner"> ' +
    '        <div class="item-title-row"> ' +
    '          <div class="item-title">' + comment.username + '</div> ' +
    '        </div> ' +
    '        <div class="item-text">' + comment.text + '</div> ' +
    '      </div> ' +
    '    </a> ' +
    '  </li>';
}
getCategoryName = (categoryID) => {
  switch (categoryID) {
    case 0:
      return "Feminino";
      break;
    case 1:
      return "Masculino";
      break;
    case 2:
      return "HxM";
      break;
    case 3:
      return "MxM";
      break;
    case 4:
      return "HxH";
      break;
  }
};
getCurrentCategory = () => {
  switch (currentCategory) {
    case 0:
      return "tab-feminino";
      break;
    case 1:
      return "tab-masculino";
      break;
    case 2:
      return "tab-casal-hetero";
      break;
    case 3:
      return "tab-casal-homoM";
      break;
    case 4:
      return "tab-casal-homoH";
      break;
  }
};
formatDate = (date) => {
  var date = new Date(),
    day = date.getDate().toString().padStart(2, '0'),
    month = (date.getMonth() + 1).toString().padStart(2, '0'), //+1 pois no getMonth Janeiro começa com zero.
    year = date.getFullYear();
  return day + "/" + month + "/" + year;
}

conditionalRedirect = (path) => {
  if (Cookies.get("nffTkn"))
    app.router.navigate(path);
  else
    app.router.navigate('/login/');
};

navigateToIndex = () => {
  app.router.navigate('/home/');
  $.each($(".footer-tab-button"), i => $($(".footer-tab-button")[i]).removeClass("tab-link-active"));
  $('.index-footer-link').addClass("tab-link-active");
};

loadPostComments = (postID, isFirstLoad) => {
  let lastID = 0;
  if (!isFirstLoad) {
    let lastPostItem = $($("#post-comment-infinity li")[$("#post-comment-infinity li").length - 1]);
    lastID = parseInt(lastPostItem.attr("id").split("post_comment_")[1]);
  }
  $.post(APIUrl + "Comment/GetComments",
    {
      postID: postID,
      lastID: lastID
    },
    (result) => {
      if (result.success) {

        if (isFirstLoad) {
          $("#post-comment-infinity .skeletonFade").remove();
          if (result.data.length == 0) {
            $("#noComment").show();
            $(".preloader").hide();
            return;
          }
        }

        if (result.data.length < 5) {
          isPostCommentsFullyLoaded = true;
          $(".preloader").hide();
        }

        result.data.forEach(comment => {
          $(mountCommentItem(comment)).appendTo('#post-comment-infinity .list');
        });
        isPostCommentsLoading = false;
        return;
      }
      app.toast.create({
        text: 'Tivemos um problema ao carregar os comentários.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    })
    .fail((e) => {
      app.toast.create({
        text: 'Tivemos um problema ao carregar os comentários.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });

};
function scrollPosts() {
  // Exit, if loading in progress
  if (!allowInfinite) return;
  //$('.tab-active .block').append('<div class="preloader infinite-scroll-preloader"></div>');
  // Set loading flag
  allowInfinite = false;
  // Emulate 1s loading
  setTimeout(function () {
    showPreLoader();
    // Reset loading flag
    if (lastPostLength == totalToGet)
      allowInfinite = true;
    if (lockCategory[currentCategory] != 0)
      return;
    lockCategory[currentCategory] = 1;
    $.post(APIUrl + "Post/GetPosts",
      {
        lastID: lastID[currentCategory],
        count: totalToGet,
        category: currentCategory,
        token: Cookies.get("nffTkn")
      },
      (result) => {
        if (result.success) {
          var tmpCategory = currentCategory;
          app.preloader.hide();
          lastPostLength = result.data.length;
          if (result.data.length == 0) {
            allowInfinite = false;
            $('.infinite-scroll-preloader').hide();
            app.toast.create({
              text: 'Por hoje é só pessoal!!!',
              position: 'top',
              closeTimeout: 2000,
            }).open();
            $("noNude").show();
            return;
          }
          result.data.forEach(post => {
            currentCategory = post.category;
            $(mountPostItem(post)).appendTo('#' + getCurrentCategory() + ' .block');
            if (lastID[currentCategory] == 0 || lastID[currentCategory] > post.postID)
              lastID[currentCategory] = post.postID;
          });
          currentCategory = tmpCategory;
          lockCategory[currentCategory] = 0;
          app.preloader.hide();
          return;
        }
        lockCategory[currentCategory] = 0;
        app.preloader.hide();
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      })
      .fail((e) => {
        app.preloader.hide();
        lockCategory[currentCategory] = 0;
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      });
    // $('.infinite-scroll-preloader').fadeOut();

  }, 100);

}
$('.infinite-scroll-content').on('infinite', function () {
  scrollPosts();
});

deletePost = (postID) => {
  app.dialog.confirm('Tem certeza que deseja apagar essa foto?', 'Apagar postagem', () => {
    showPreLoader();
    $.post(APIUrl + "Post/DeletePost",
      {
        token: Cookies.get("nffTkn"),
        postID: postID
      },
      (result) => {
        app.preloader.hide();
        if (result.success) {
          app.toast.create({
            text: 'Post apagado com sucesso.',
            position: 'top',
            closeTimeout: 2000,
          }).open();
          $('#myNudesPost_' + postID).fadeOut(600, function () { $(this).remove(); });
          return;
        }
        app.preloader.hide();
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      }).fail((e) => {
        app.preloader.hide();
        app.toast.create({
          text: 'Tivemos um problema. Tente novamente.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
      });
  });
};

likePost = (postID) => {
  if (!e) var e = window.event;
  e.cancelBubble = true;
  if (e.stopPropagation) e.stopPropagation();

  if (!Cookies.get("nffTkn")) {
    app.toast.create({
      text: 'É necessário estar logado.',
      position: 'top',
      closeTimeout: 2000,
    }).open();
    return;

  }
  showPreLoader();
  $.post(APIUrl + "Post/HotPost",
    {
      token: Cookies.get("nffTkn"),
      postID: postID
    },
    (result) => {
      app.preloader.hide();
      if (result.success) {
        if (result.data) {
          app.toast.create({
            text: 'Like!',
            position: 'top',
            closeTimeout: 2000,
          }).open();
          $("#like" + postID).removeClass('ion-ios-heart-empty');
          $("#like" + postID).addClass('ion-ios-heart');
          $("#countLike" + postID).html(($("#countLike" + postID).html() * 1) + 1);
          $("#total_likes").html(($("#total_likes").html() * 1) + 1);
          $("#heartType").removeClass('ion-ios-heart-empty');
          $("#heartType").addClass('ion-ios-heart');
        } else {
          $("#heartType").removeClass('ion-ios-heart-empty');
          $("#heartType").addClass('ion-ios-heart');
          app.toast.create({
            text: 'Você já curtiu essa foto ',
            position: 'top',
            closeTimeout: 2000,
          }).open();
        }
        return;
      }
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    }).fail((e) => {
      app.preloader.hide();
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
};

reportPost = (reportType) => {

  if (!Cookies.get("nffTkn")) {
    app.toast.create({
      text: 'É necessário estar logado.',
      position: 'top',
      closeTimeout: 2000,
    }).open();
    return;

  }
  showPreLoader();
  $("#pstID" + $("#postID").val()).hide();
  $.post(APIUrl + "Post/Report",
    {
      token: Cookies.get("nffTkn"),
      postID: $("#postID").val(),
      reportType: reportType
    },
    (result) => {
      app.preloader.hide();
      if (result.success) {
        app.toast.create({
          text: 'Notificado, agradecemos sua contribuição.',
          position: 'top',
          closeTimeout: 2000,
        }).open();
        navigateToIndex();
        return;
      }
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    }).fail((e) => {
      app.preloader.hide();
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
};
commentPost = () => {
  if (!Cookies.get("nffTkn")) {
    app.toast.create({
      text: 'É necessário estar logado.',
      position: 'top',
      closeTimeout: 2000,
    }).open();
    return;
  }
  showPreLoader();
  $.post(APIUrl + "Comment/CreateComment",
    {
      token: Cookies.get("nffTkn"),
      postID: $("#postID").text(),
      text: $("#postCommentText").val()
    },
    (result) => {
      app.preloader.hide();
      if (result.success) {
        app.toast.create({
          text: 'Feito!',
          position: 'top',
          closeTimeout: 2000,
        }).open();
        $("#noComment").hide();

        let comment = result.data;
        comment.avatar = JSON.parse(Cookies.get("nff_profile")).avatar;

        $(mountCommentItem(comment)).appendTo('#post-comment-infinity .list');
        $("#postCommentText").val('');
        return;
      }
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    }).fail((e) => {
      app.preloader.hide();
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
};

loadPost = (postID) => {
  $.post(APIUrl + "Post/GetPublication",
    {
      postID: postID,
      token: Cookies.get("nffTkn")
    },
    (result) => {
      if (result.success) {
        $('#listComments').html('');
        $('#skeleton').remove();
        $('#post-image').attr('src', APIUrl + '/Posts/' + result.data.post.filename)
        $('#countComments').html(result.data.comments.length);
        if (result.data.post.likes > 10) {
          $('#isHOT').show();
        }
        if (result.data.post.hasLike) {
          $("#heartType").removeClass('ion-ios-heart-empty');
          $("#heartType").addClass('ion-ios-heart');
        }
        $("#post_user_avatar").attr("src", APIUrl + "avatars/avatar_" + result.data.post.userID + ".png");
        $("#post_username").text(result.data.post.username);
        $("#total_likes").html(result.data.post.likes);
        $("#post_description").text(result.data.post.description);
        $("#post_date").text(formatDate(result.data.post.postDate));
        if (result.data.comments.length == 0) {
          $('#listComments').html('<div id="noComments">Sem comentários, seja o primeiro a comentar!</div>');
        }
        result.data.comments.forEach(comment => {
          $(mountCommentItem(comment)).appendTo('#listComments');
        });
        return;
      }
      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    })
    .fail((e) => {
      app.router.navigate("/");
      app.toast.create({
        text: 'Tivemos um problema. Tente novamente.',
        position: 'top',
        closeTimeout: 2000,
      }).open();
    });
};
MF = () => {
  currentCategory = 0;
  $("#MC").removeClass('tab-link-active');
  allowInfinite = true;
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};
MH = () => {
  currentCategory = 1;
  $("#MC").removeClass('tab-link-active');
  allowInfinite = true;
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};
MC = () => {
  currentCategory = 2;
  allowInfinite = true;
  $("#MC").addClass('tab-link-active');
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};
Mhxm = () => {
  currentCategory = 2;
  allowInfinite = true;
  $("#MC").addClass('tab-link-active');
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};
Mmxm = () => {
  currentCategory = 3;
  allowInfinite = true;
  $("#MC").addClass('tab-link-active');
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};
Mhxh = () => {
  currentCategory = 4;
  allowInfinite = true;
  $("#MC").addClass('tab-link-active');
  $('.infinite-scroll-content').on('infinite', function () {
    scrollPosts();
  });
  loadNudes();
};