import React, {Component} from 'react';
import DonutChart from 'react-donut-chart';

class Statistics extends Component {
  constructor() {
    super();

    this.state = {
      card: [],
      title: 'Gevoel'
    }
  }

  componentDidMount() {
    fetch('/api/v1/statistics')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson
        })
        console.log(this.state.card);
      })
  }


  render(){

    let arrayGevoel = [];
    let arrayLeesbaar = [];
    let arrayInteressant = [];
    let arrayDuidelijkheid = [];

    {this.state.card.map((item) =>
        {if((item['oneWord']) == "Gevoel"){
            {item['feedback'].map((item2) =>
               arrayGevoel.push(item2['count'])
            )}
        }else if((item['oneWord']) == "Leesbaar"){
            {item['feedback'].map((item2) =>
                arrayLeesbaar.push(item2['count'])
            )}
        }else if((item['oneWord']) == "Interessant"){
            {item['feedback'].map((item2) =>
                arrayInteressant.push(item2['count'])
            )}
        }else if((item['oneWord']) == "Duidelijkheid"){
            {item['feedback'].map((item2) =>
                arrayDuidelijkheid.push(item2['count'])
            )}
        }}
    )}

    function generateValues(arrayGevoel){

        var data = [];
        var colors = ['#ff0043','#77c6a0','#304964'];
      
        {arrayGevoel.map((innerFeedbackValue, innerFeedbackIndex) =>
          data.push({value: innerFeedbackValue, label: "hello", isEmpty: false, colorFunction: colors[innerFeedbackIndex], strokeColor: colors[innerFeedbackIndex]})
        )}
        console.log(arrayGevoel)
        return data;
       }

       function generateValues2(arrayLeesbaar){

        var data = [];
        var colors = ['#ff0043','#77c6a0','#304964'];
      
        {arrayLeesbaar.map((innerFeedbackValue, innerFeedbackIndex) =>
          data.push({value: innerFeedbackValue, clickToggle: false, label: "hello", width: 500, height: 500, isEmpty: false, colorFunction: colors[innerFeedbackIndex], strokeColor: colors[innerFeedbackIndex]})
        )}
        console.log(arrayLeesbaar)
        return data;
       }

       function generateValues3(arrayInteressant){

        var data = [];
        var colors = ['#ff0043','#77c6a0','#304964'];
      
        {arrayInteressant.map((innerFeedbackValue, innerFeedbackIndex) =>
          data.push({label: "hello", value: innerFeedbackValue,  height: 500, width: 500, clickToggle: false,  isEmpty: false, colorFunction: colors[innerFeedbackIndex], strokeColor: '#304964'})
        )}
        console.log(arrayInteressant)
        return data;
       }

       function generateValues4(arrayDuidelijkheid){

        var data = [];
        var colors = ['#ff0043','#77c6a0','#304964'];
      
        {arrayDuidelijkheid.map((innerFeedbackValue, innerFeedbackIndex) =>
          data.push({value: innerFeedbackValue, clickToggle: false, label: "hello", width: 500, height: 500, isEmpty: false, colors: colors, strokeColor: colors[innerFeedbackIndex]})
        )}
          console.log(arrayDuidelijkheid)
        return data;
       }


    return (
        <div className="statisticsContentContainer">
            <div className="row statisticsFilter">
                <div className="col s2 offset-s10 statisticsLabel">Statestieken</div>
            </div>
            <div className="row allCharts">
                <div className="col s6 feedbackChartOne chart">
                    <DonutChart
                        data={generateValues((arrayGevoel))}
                    />
                    <div className="titleStat">Gevoel</div>
                </div>
                <div className="col s6 feedbackChartTwo chart">
                    <DonutChart
                        data={generateValues2((arrayLeesbaar))}
                    />
                    <div className="titleStat">Leesbaarheid</div>
                </div>
            </div>
            <div className="row allCharts">
                <div className="col s6 feedbackChartThree chart">
                    <DonutChart
                        data={generateValues3((arrayInteressant))}
                    />
                    <div className="titleStat">Interesse</div>
                </div>
                <div className="col s6 feedbackChartThree chart">
                    <DonutChart
                        data={generateValues4((arrayDuidelijkheid))}
                    />
                    <div className="titleStat">Duidelijkheid</div>
                </div>
           

            </div>
        </div>
    )
  }
}
export default Statistics;