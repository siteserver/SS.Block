var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/settings/',
  params: {
    siteId: utils.getQueryInt('siteId')
  },
  withCredentials: true
});

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  configInfo: null,
  areas: null,
  blockAreas: null,
};

var methods = {
  getConfigInfo: function(configInfo) {
    configInfo.blockMethod = configInfo.blockMethod ? configInfo.blockMethod : 'RedirectUrl';
    configInfo.redirectUrl = configInfo.redirectUrl ? configInfo.redirectUrl : '';
    configInfo.warning = configInfo.warning ? configInfo.warning : '';
    configInfo.password = configInfo.password ? configInfo.password : '';

    return configInfo;
  },

  apiGetSettings: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.configInfo = $this.getConfigInfo(res.value);
      $this.areas = res.areas;
      $this.blockAreas = res.blockAreas;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiSubmitSettings: function () {
    var $this = this;

    var payload = {
      type: this.pageType
    };
    if (this.pageType === 'isBlock') {
      payload.isBlock = this.configInfo.isBlock;
    } else if (this.pageType === 'isBlockAll') {
      payload.isBlockAll = this.configInfo.isBlockAll;
      payload.blockAreas = this.blockAreas;
    } else if (this.pageType === 'blockMethod') {
      payload.blockMethod = this.configInfo.blockMethod;
      payload.redirectUrl = this.configInfo.redirectUrl;
      payload.warning = this.configInfo.warning;
      payload.password = this.configInfo.password;
    }

    utils.loading(true);
    $api.post('', payload).then(function (response) {
      var res = response.data;

      $this.configInfo = $this.getConfigInfo(res.value);
      $this.areas = res.areas;
      $this.blockAreas = res.blockAreas;

      $this.pageType = 'list';
      swal2({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmitSettings();
      }
    });
  },

  btnNavClick: function (pageName) {
    utils.loading(true);
    location.href = utils.getPageUrl(pageName);
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGetSettings();
  }
});
