import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import './index.css';
import MainLanding from './components/MainLanding';
import configureStore from './redux/configureStore';

const store = configureStore();
const App = () => (
    <Provider store={store}>
        <MainLanding />
    </Provider>
);

ReactDOM.render(<App />, document.getElementById('root'));
