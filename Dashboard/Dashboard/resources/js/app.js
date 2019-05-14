require('./bootstrap');
import React from 'react';
import { render } from 'react-dom';
import { Router, Route, browserHistory } from 'react-router';

import Master from './components/Master';
import CreateItem from './components/CreateItem';
import DisplayItem from './components/DisplayItem';



render(
  <Router history={browserHistory}>
        <Route path="/" component={Master} >
            <Route path="/add-item" component={CreateItem} />
            <Route path="/see-item" component={DisplayItem} />
        </Route>
    </Router>,
        document.getElementById('example'));