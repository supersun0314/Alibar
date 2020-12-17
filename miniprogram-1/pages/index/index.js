//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    account:11,
    password:22
  },

  accountHandler:function(e)
  {
    console.log(this.data.account);
  },

  passwordHandler:function(e)
  {
    console.log(this.data.password)
  },

  OnLoginBtnClick:function(e)
  {
    console.log("登陆")
  },

  OnRegisterClick:function(e)
  {
    console.log("注册")
  }
})
