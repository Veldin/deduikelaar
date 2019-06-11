import React, {Component} from 'react';
import toastr from 'toastr';
import Card from './Card/Card';

class Overview extends Component {
  constructor() {
    super();

    this.state = {
      card: []
    }
  }

  deleteItem(id) {
    fetch('/api/v1/story/'+id,{
      method: 'DELETE',
    })
    .then(response => response.json())
    .then(response => {
      if(response['response'] == "success"){
        toastr.success('Het item is verwijderd!', '', {positionClass: "toast-bottom-right", timeOut: 40000})
        this.setState({
          card: this.state.card.filter(s => s.storyId !== id)
        });
      }else{
        toastr.warning('dit item is al verwijderd', '', {positionClass: "toast-bottom-right", timeOut: 40000})
      }
    })
  }

  componentDidMount() {
    fetch('/api/v1/overview')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson
        })
      })
  }

  render(){
    return (
      <div className="overviewContentContainer">
        <div className="row overviewFilter">
          <div className="col s2 overviewLabel">Alle Items</div>
          <div className="col s10 overviewSwitch">
            <div className="switch">
              <label>
                Alleen Actief
                <input type="checkbox" defaultChecked></input>
                <span className="lever"></span>
                Alle
              </label>
            </div>
          </div>
        </div>
        <div className="row overviewCards">
          <div className="cards-container">
            {this.state.card.map((item, key) =>
              <Card 
                onDelete={this.deleteItem.bind(this)}
                key={key} 
                active={item.active}
                storyID={item.storyId} 
                title={item.title} 
                active={item.active}
                cardInfo={[this.state.card]}
              />
            )}
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;