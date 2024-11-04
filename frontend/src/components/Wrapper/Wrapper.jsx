import classes from './Wrapper.module.css';

export default function Wrapper({ children }) {
    return <div className={classes.wrapper}> {children}</div>;
}
