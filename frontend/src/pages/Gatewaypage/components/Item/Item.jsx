import classes from './Item.module.css';

const Item = ({ title, descr }) => {
    return (
        <div className={classes.bold}>
            <span>{title}:</span>
            {' ' + descr}
        </div>
    );
};

export default Item;
