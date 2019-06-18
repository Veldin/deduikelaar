import React, {Component} from 'react';
import toastr from 'toastr';
import Card from './Card/Card';

// Shows the individual stories in cards with all the required information therein
class Overview extends Component {
  constructor() {
    
    // Execute constructor of component
    super();

    // Set default values
    this.state = {
      card: [],
      modal: null,
      modalContent: "",
      selectValue: ""
    }
  }

  // This function gets executed when the page loads
  componentDidMount(sort=null) {

    // Sets the initial state of the model that shows the examples of stories
    this.state.modal = M.Modal.init(document.getElementById('story_example'), {opacity: 0});

    // Determines in what order stories are displayed
    switch(sort) {
      case 'date':

        // Gets stories ordered by date from the API
        fetch('/api/v1/overview?order=created_at:desc')
        .then((response) => response.json())
        .then((responseJson) => {
          this.setState({ 
            card: responseJson
          })
        })
        break;
      case 'alpha':

        // Gets stories ordered by alphabet from the API
        fetch('/api/v1/overview?order=title:asc')
        .then((response) => response.json())
        .then((responseJson) => {
          this.setState({ 
            card: responseJson
          })
        })
        break;
      default:

        // Gets stories
        fetch('/api/v1/overview')
        .then((response) => response.json())
        .then((responseJson) => {
          this.setState({ 
            card: responseJson
          })
        })
    }
  }

  // Handles active/nonactive states showing the stories based on the state of the switch
  toggleSwitch() {
    var elements = document.getElementsByClassName('notActive'); // Get all non active elements
    
    var i;
    for (i = 0; i < elements.length; i++) { 

      // If display is none then show the card else hide the card
      if (elements[i].parentNode.style.display === "none") {
        elements[i].parentNode.style.display = "block";
      } else {
        elements[i].parentNode.style.display = "none";
      }
    }
  }

  // Removes story from DOM and Database based on given id
  deleteItem(id) {

    // Calls API function to delete a story
    fetch('/api/v1/story/'+id,{
      method: 'DELETE',
    })
    .then(response => response.json()) // Parses response to JSON
    .then(response => {

      // Give the user feedback
      if(response['response'] == "success"){
        toastr.success('Het item is verwijderd!', '')
        this.setState({
          // Filter out the removed card from the card list
          card: this.state.card.filter(s => s.storyId !== id)
        });
      }else{
        toastr.warning('dit item is al verwijderd', '')
      }
    })
  }

  // Calls componentDidMount with the value of select box to sort the card list
  handleSorting(event){
    this.componentDidMount(event.target.value)
  };

  // Open modal and load data based on storyID
  showItem(id) {
    this.setState({
      modalContent: "<iframe onload=\"this.style.display='block';document.getElementById('spinner').style.display = 'none';\" class=\"modal-container\" src=\"/api/v1/story/"+id+"/preview?overview\" frameborder=\"0\" style='display: none;'></iframe><svg id=\"spinner\" aria-hidden=\"true\" focusable=\"false\" data-prefix=\"fas\" data-icon=\"spinner\" class=\"svg-inline--fa fa-spinner fa-w-32 fa-spin fa-pulse \" role=\"img\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 512 512\"><path fill=\"currentColor\" d=\"M304 48c0 26.51-21.49 48-48 48s-48-21.49-48-48 21.49-48 48-48 48 21.49 48 48zm-48 368c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.49-48-48-48zm208-208c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.49-48-48-48zM96 256c0-26.51-21.49-48-48-48S0 229.49 0 256s21.49 48 48 48 48-21.49 48-48zm12.922 99.078c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48c0-26.509-21.491-48-48-48zm294.156 0c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48c0-26.509-21.49-48-48-48zM108.922 60.922c-26.51 0-48 21.49-48 48s21.49 48 48 48 48-21.49 48-48-21.491-48-48-48z\"></path></svg>"
    });
    this.state.modal.open();
  }

  // Renders the overview with the card components as well as the controls
  render(){
    return (
      <div className="overviewContentContainer">
        <div className="row overviewFilter">
          <div className="col s5 m2 l8 overviewLabel">Alle Items</div>
          <div className="input-field col hide-on-small-only m2 l2 sortDropdown">
            {/* Calls handleSorting function when switching */}
            <select onChange={this.handleSorting.bind(this)}>
              <option value="">Sorteer op..</option>
              <option value="date">Datum (nieuw/oud)</option>
              <option value="alpha">Alfabetisch</option>
            </select>
          </div>
          <div className="col s7 m8 l2 overviewSwitch">
            {/* Calls toggleSwitch when clicking the switch */}
            <div className="switch" onChange={this.toggleSwitch.bind(this)}>
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
            {/* Loop through array of cards and load a card component for each with given props */}
            {this.state.card.map((item, key) =>
              <Card 
                onDelete={this.deleteItem.bind(this)}
                onShow={this.showItem.bind(this)}
                key={key} 
                active={item.active}
                storyID={item.storyId} 
                title={item.title} 
                active={item.active}
                cardInfo={[this.state.card]}
              >
              </Card>
            )}
          </div>
        </div>

        {/* HTML of the modal */}
        <div id="story_example" className="modal modal-fixed-footer preview">
          <div className="modal-content" dangerouslySetInnerHTML={{__html: this.state.modalContent}}></div>
          
          <div className="modal-footer">
            <a href="#!" className="modal-close btn waves-effect waves-light">SLUIT</a>
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;