
import React from 'react';

const StockListing = ({ stockPrice }) => {
    const styles = {
        mainRow: {
            'border': '1px solid black',
            'width': '300px',
            'display': 'flex',
            'marginBottom': '30px',
            'height': '60px',
            'alignItems': 'center',
            'backgroundColor': stockPrice.priceChange > 0 ? '#bdecb6' : '#ff9999'
        },
        symbolText: {
            'fontSize': '18pt',
            'marginLeft': '7px',
            'fontWeight': 'bold',
            'display': 'block',
            'flexGrow': 1
        },
        symbolPrice: {
            'fontSize': '14pt',
            'marginRight': '15px',
            'display': 'block'
        },
        symbolPriceChange: {
            'fontSize': '10pt',
            'marginRight': '7px',
            'display': 'block',
            'width': '50px'
        }
    };

    return (
        <div style={styles.mainRow}>
            <div style={styles.symbolText}>{stockPrice.symbol}</div>
            <div style={styles.symbolPrice}>{stockPrice.newPrice}</div>
            <div style={styles.symbolPriceChange}>{stockPrice.priceChange}</div>
        </div>
    );
};

export default StockListing;