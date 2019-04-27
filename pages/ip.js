var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/ip/',
  params: {
    siteId: utils.getQueryInt('siteId')
  },
  withCredentials: true
});

var data = {
  pageLoad: false,
  pageAlert: null,
  ipAddress: null,
  isAllowed: null,
  areaInfo: null
};

var methods = {
  apiGetIpAddress: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.ipAddress = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiQuery: function () {
    var $this = this;

    utils.loading(true);
    $api.post('', {
      ipAddress: this.ipAddress
    }).then(function (response) {
      var res = response.data;
      $this.isAllowed = res.value;
      $this.areaInfo = res.areaInfo;
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
        $this.apiQuery();
      }
    });
  },

  btnNavClick: function (pageName) {
    utils.loading(true);
    location.href = utils.getPageUrl(pageName);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGetIpAddress();
  }
});
