var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/analysis/",
  params: {
    siteId: utils.getQueryInt("siteId")
  },
  withCredentials: true
});

var methods = {
  apiGetAnalysis: function() {
    var $this = this;

    $api
      .get("")
      .then(function(response) {
        var res = response.data;

        $this.days = res.days;
        $this.count = res.count;
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.pageLoad = true;
      });
  },

  btnNavClick: function(pageName) {
    utils.loading(true);
    location.href = utils.getPageUrl(pageName);
  }
};

Vue.component("line-chart", {
  extends: VueChartJs.Line,
  mounted: function() {
    this.renderChart(
      {
        labels: this.$root.days,
        datasets: [
          {
            label: "拦截次数",
            backgroundColor: "#f87979",
            data: this.$root.count
          }
        ]
      },
      {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true
              }
            }
          ]
        }
      }
    );
  }
});

var $vue = new Vue({
  el: "#main",
  data: {
    pageLoad: false,
    pageAlert: null,
    labels: null,
    data: null
  },
  methods: methods,
  created: function() {
    this.apiGetAnalysis();
  }
});
