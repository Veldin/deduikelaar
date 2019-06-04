import React, {Component} from 'react';
import DonutChart from "react-svg-donut-chart";
import Card from './Card/Card';

class Statistics extends Component {
  constructor() {
    super();

    this.state = {
      card: [],
      title: 'Gevoel'
    }
  }

  componentDidMount() {
    fetch('/api/v1/overview')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson
        })
        console.log(this.state.card);
      })
  }

  render(){
 

    {this.state.card.map((item) =>
        {item['feedback'].map((item2) =>
            {if((item2['oneWord']) == "Gevoel"){
                console.log("hoi ");
            }else{
                console.log('doet neit');
            }}
        )}
    )}
    
    return (
        <div className="statisticsContentContainer">
            <div className="row statisticsFilter">
                <div className="col s2 statisticsLabel">Statestieken</div>
                <div className="col s10"></div>
            </div>
            <div className="row allCharts">
                <div className="col s6 feedbackChartOne chart">
                    <DonutChart
                        data={[
                            {
                            stroke: '#ff0043',
                            strokeWidth: 6,
                            value: 33
                            },
                            {
                            stroke: '#77c6a0',
                            strokeWidth: 6,
                            value: 60
                            },
                            {
                            stroke: '#304964',
                            strokeWidth: 6,
                            value: 30
                            }
                        ]}
                    />
                    <div className="titleStat">Gevoel</div>
                </div>
                <div className="col s6 feedbackChartTwo chart">
                    <DonutChart
                        data={[
                            {
                            stroke: '#ff0043',
                            strokeWidth: 6,
                            value: 10
                            },
                            {
                            stroke: '#77c6a0',
                            strokeWidth: 6,
                            value: 70
                            },
                            {
                            stroke: '#304964',
                            strokeWidth: 6,
                            value: 30
                            }
                        ]}
                    />
                    <div className="titleStat">Leesbaarheid</div>
                </div>
            </div>
            <div className="row allCharts">
                <div className="col s6 feedbackChartThree chart">
                    <DonutChart
                        data={[
                            {
                            stroke: '#ff0043',
                            strokeWidth: 6,
                            value: 100
                            },
                            {
                            stroke: '#77c6a0',
                            strokeWidth: 6,
                            value: 60
                            },
                            {
                            stroke: '#304964',
                            strokeWidth: 6,
                            value: 90
                            }
                        ]}
                    />
                    <div className="titleStat">Interesse</div>
                </div>
                <div className="col s6 feedbackChartThree chart">
                    <DonutChart
                        data={[
                            {
                            stroke: '#ff0043',
                            strokeWidth: 6,
                            value: 100
                            },
                            {
                            stroke: '#77c6a0',
                            strokeWidth: 6,
                            value: 60
                            },
                            {
                            stroke: '#304964',
                            strokeWidth: 6,
                            value: 90
                            }
                        ]}
                    />
                    <div className="titleStat">Duidelijkheid</div>
                </div>
           

            </div>
        </div>
    )
  }
}
export default Statistics;