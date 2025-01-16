import LoadingSpin from 'react-loading-spin';

const Loader = () => {
    return (
        <div style={{ display: 'flex', justifyContent: 'center' }}>
            <LoadingSpin />
        </div>
    );
};

export default Loader;
