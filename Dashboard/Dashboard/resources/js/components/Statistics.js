import React, {Component} from 'react';

class Statistics extends Component {
  constructor() {
    super();

    this.state = {
      card: [],
      canvasData: {},
      canvasSave: {}
    }
  }

  componentDidMount() {
    fetch('/api/v1/statistics')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson
        });

          responseJson.map((item, index) => {

              this.createChart('canvas'+index, item);

          })
      });

      const script = document.createElement("script");
      script.src = "js/hack-donutchart.js";
      script.async = true;
      document.body.appendChild(script);



  }

  createChart(id, data){

      var total = parseInt(data['count']);
      var offset = 0;
      var colors = ['#ff0043','#77c6a0','#304964'];
      colors.sort(() => Math.random() - 0.5);

      var c = document.getElementById(id);
      c.width = 500;
      c.height = 500;
      var ctx = c.getContext("2d");
      ctx.beginPath();
      ctx.lineWidth = 40;
      ctx.font = "20px Arial";
      ctx.fillStyle = "#FFFFFF";
      ctx.textAlign = "center";
      ctx.fillText(data['question'], 250, 250);
      ctx.font = "16px Arial";
      var size = 180;
      var canvasData = [];
      data['feedback'].map(function(fb, fbIndex){
          ctx.beginPath();

          var end = (parseInt(fb['count']) / total) * 2;
          end += offset;

          ctx.arc(c.width/2, c.height/2, size, offset * Math.PI, end * Math.PI);
          ctx.strokeStyle = colors[fbIndex];
          ctx.stroke();

          var loc = (offset + ((end-offset)/2)) * Math.PI;
          var xLoc = size*Math.cos(loc);
          var yLoc = size*Math.sin(loc);

          canvasData[fbIndex] = {
              x: xLoc,
              y: yLoc,
              answer: fb['answer'],
              count: fb['count']
          };
          offset = end;
      });
      if(typeof this.state.canvasData === "undefined"){
          this.state.canvasData = {};

      }
      this.state.canvasData[id] = canvasData;
      this.state.canvasSave[id] = ctx.getImageData(0, 0, c.width, c.height);

  }

  mouseEnter (e) {
      var c = e.target;
      var ctx = c.getContext("2d");
      ctx.beginPath();
      ctx.lineWidth = 40;

      this.state.canvasData[c.id].map(function(item){
          ctx.beginPath();
          var xLoc = item.x;
          var yLoc = item.y;

          switch (item.answer) {
              case "\\u1F603":
                  var img = new Image();
                  img.src = "images/sprites/happy.gif";
                  img.onload = function(){
                      ctx.drawImage(this, 235 + xLoc, 225 + yLoc, 30, 30);
                  };
                  break;
              case "\\u1F620":
                  var img = new Image();
                  img.src = "images/sprites/angry.gif";
                  img.onload = function(){
                      ctx.drawImage(this, 235 + xLoc, 225 + yLoc, 30, 30);
                  };
                  break;
              case "\\u1F622":
                  var img = new Image();
                  img.src = "images/sprites/sad.gif";
                  img.onload = function(){
                      ctx.drawImage(this, 235 + xLoc, 225 + yLoc, 30, 30);
                  };
                  break;
              default:
                  ctx.fillText(item.answer, 250 + xLoc, 250 + yLoc);
          }
          ctx.fillText(item.count, 250 + xLoc, 280 + yLoc);
      });
  }

  mouseLeave (e) {
      var c = e.target;
      var ctx = c.getContext("2d");
      ctx.putImageData(this.state.canvasSave[c.id], 0, 0);
  }



  render(){
      return (
              <div className="statisticsContentContainer">
                  <div className="row statisticsFilter">
                      <div className="col s12 m2 l2 offset-m10 offset-l10 statisticsLabel">Statestieken</div>
                  </div>
                  <div className="row allCharts">
                  {this.state.card.map((item, index) => {


                      return <div className="col s12 m6 l6 chart" key={index}>
                          <canvas id={"canvas"+index} onMouseEnter={this.mouseEnter.bind(this)} onMouseLeave={this.mouseLeave.bind(this)}></canvas>
                      </div>

                  })}
                  </div>
              </div>
      )
  }
}
export default Statistics;