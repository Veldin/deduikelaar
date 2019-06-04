require('./materialize');
import React from 'react';
import { render } from 'react-dom';
import { Router, Route, browserHistory } from 'react-router';

import Master from './components/Master';
import Overview from './components/Overview';
import CreateItem from './components/CreateItem';
import Statistics from './components/Statistics';



render(
  	<Router history={browserHistory}>
        <Route path="/" component={Master} >
        	<Route path="/overview" component={Overview} />
            <Route path="/add-item" component={CreateItem} />
            <Route path="/statistics" component={Statistics} />
        </Route>
    </Router>,
document.getElementById('superContainer'));