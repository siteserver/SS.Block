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
  isEnabled: null,
  isAllChannels: null,
  channels: null,
  blockChannels: null,
  isAllAreas: null,
  areas: null,
  blockAreas: null,
  blockMethod: null,
  redirectUrl: null,
  warning: null,
  password: null,
};

var methods = {
  loadSettings: function() {
    this.isEnabled = !!this.configInfo.isEnabled;
    this.isAllChannels = !!this.configInfo.isAllChannels;
    this.isAllAreas = !!this.configInfo.isAllAreas;
    this.blockMethod = this.configInfo.blockMethod ? this.configInfo.blockMethod : 'RedirectUrl';
    this.redirectUrl = this.configInfo.redirectUrl ? this.configInfo.redirectUrl : '';
    this.warning = this.configInfo.warning ? this.configInfo.warning : '';
    this.password = this.configInfo.password ? this.configInfo.password : '';
  },

  apiGetSettings: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.configInfo = res.value;
      $this.loadSettings();
      $this.areas = res.areas;
      $this.blockAreas = res.blockAreas;
      $this.channels = res.channels;
      $this.blockChannels = res.blockChannels;
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
    if (this.pageType === 'isEnabled') {
      payload.isEnabled = this.isEnabled;
    } else if (this.pageType === 'isAllChannels') {
      payload.isAllChannels = this.isAllChannels;
      payload.blockChannels = this.blockChannels;
    } else if (this.pageType === 'isAllAreas') {
      payload.isAllAreas = this.isAllAreas;
      payload.blockAreas = this.blockAreas;
    } else if (this.pageType === 'blockMethod') {
      payload.blockMethod = this.blockMethod;
      payload.redirectUrl = this.redirectUrl;
      payload.warning = this.warning;
      payload.password = this.password;
    }

    utils.loading(true);
    $api.post('', payload).then(function (response) {
      var res = response.data;

      $this.configInfo = res.value;
      $this.loadSettings();
      $this.areas = res.areas;
      $this.blockAreas = res.blockAreas;
      $this.channels = res.channels;
      $this.blockChannels = res.blockChannels;

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
